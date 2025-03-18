using MatrixResponsibility.Common;
using MatrixResponsibility.Common.DTOs.Response;
using MatrixResponsibility.Common.Interafaces;
using MatrixResponsibility.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MatrixResponsibility.Services
{
    public class DataImportService
    {
        private readonly AppDbContext _context;
        private readonly ILDAPAuthenticationService _ldapService;
        private List<UserInfo> _allSotrudniks;

        public DataImportService(AppDbContext context, ILDAPAuthenticationService ldapService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _ldapService = ldapService ?? throw new ArgumentNullException(nameof(ldapService));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Установка лицензии для EPPlus
        }

        public async Task ImportProjects(string filePath)
        {
            //если есть хоть 1 проект то импорт не требуется
            if (await _context.Projects.AnyAsync()) return;

            // Проверка существования файла
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден по указанному пути.", filePath);
            }

            // Загрузка данных сотрудников из LDAP
            _allSotrudniks = await _ldapService.GetAllSotrudniks();
            if (_allSotrudniks == null || !_allSotrudniks.Any())
            {
                throw new InvalidOperationException("Не удалось загрузить список сотрудников из LDAP.");
            }

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets["МО ГП"];
            if (worksheet == null)
            {
                throw new ArgumentException("Лист 'МО ГП' не найден в файле.");
            }

            var rowCount = worksheet.Dimension?.Rows ?? 0;
            if (rowCount < 4)
            {
                throw new InvalidOperationException("Лист 'МО ГП' пуст или содержит недостаточно строк.");
            }

            var columnCount = Math.Min(21, worksheet.Dimension?.Columns ?? 0); // Ограничиваем до колонки U (21)
            if (columnCount < 21)
            {
                throw new InvalidOperationException("Лист 'МО ГП' содержит недостаточно колонок (ожидается минимум 21).");
            }

            // Начинаем транзакцию
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Начинаем с 4-й строки (пропускаем заголовки)
                for (int row = 4; row <= rowCount; row++)
                {
                    // Проверяем, пуста ли ячейка с объектом (C) и БКП (A)
                    var objectCell = worksheet.Cells[row, 3].Text?.Trim(); // Объект (C)
                    var bkpCell = worksheet.Cells[row, 1].Text?.Trim(); // БКП (A)
                    if (string.IsNullOrEmpty(objectCell) || string.IsNullOrEmpty(bkpCell))
                    {
                        continue; // Пропускаем пустые строки
                    }
                    if(objectCell=="Головинский 22.2")
                    {
                    }
                    var project = new Project
                    {
                        ProjectName = objectCell,
                        MarketingName = worksheet.Cells[row, 20].Text?.Trim(), // Маркетинговое название (T)
                        ObjectAddress = worksheet.Cells[row, 21].Text?.Trim(), // Адрес объекта (U)
                        GIPId = await GetOrCreateUserId(worksheet.Cells[row, 4].Text?.Trim()), // ГИП (D)
                        AssistantGIPId = await GetOrCreateUserId(worksheet.Cells[row, 5].Text?.Trim()), // ПомГИП (E)
                        GAPId = await GetOrCreateUserId(worksheet.Cells[row, 6].Text?.Trim()), // ГАП (F)
                        GKPId = await GetOrCreateUserId(worksheet.Cells[row, 7].Text?.Trim()), // ГКП (G)
                        AB = worksheet.Cells[row, 8].Text?.Trim(), // АБ (H)
                        GPId = await GetOrCreateUserId(worksheet.Cells[row, 9].Text?.Trim()), // ГП (I)
                        EOMId = await GetOrCreateUserId(worksheet.Cells[row, 10].Text?.Trim()), // ЭОМ (J)
                        SSId = await GetOrCreateUserId(worksheet.Cells[row, 11].Text?.Trim()), // СС (K)
                        AKId = await GetOrCreateUserId(worksheet.Cells[row, 12].Text?.Trim()), // АК (L)
                        ResponsibleId = await GetOrCreateUserId(worksheet.Cells[row, 13].Text?.Trim()), // Ответственный (M)
                        InternalMeeting = worksheet.Cells[row, 14].Text?.Trim(), // Внутреннее совещание (N)
                        ReportStatus = worksheet.Cells[row, 15].Text?.Trim(), // Статус отчета (O)
                        GPZUDate = ParseDateTime(worksheet.Cells[row, 16].Text?.Trim()), // Дата ГПЗУ (P)
                        Customer = worksheet.Cells[row, 17].Text?.Trim(), // Заказчик (Q)
                        StartPermissionLetter = worksheet.Cells[row, 18].Text?.Trim(), // Письмо о разрешении старта (R)
                        BKPId = await FindBKP(bkpCell) // БКП (A)
                    };

                    _context.Projects.Add(project);
                }

                // Сохраняем изменения в рамках транзакции
                await _context.SaveChangesAsync();

                // Фиксируем транзакцию
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // В случае ошибки откатываем транзакцию
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Ошибка при импорте данных. Транзакция откатана.", ex);
            }
        }

        public async Task ImportAreasAndYers(string filePath)
        {
            if (!await _context.Projects.AnyAsync()) return;

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден по указанному пути.", filePath);
            }

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets["Объекты"];
            if (worksheet == null)
            {
                throw new ArgumentException("Лист 'Объекты' не найден в файле.");
            }

            var rowCount = worksheet.Dimension?.Rows ?? 0;
            if (rowCount < 4)
            {
                throw new InvalidOperationException("Лист 'Объекты' пуст или содержит недостаточно строк.");
            }

            // Начинаем транзакцию
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var projects = await _context.Projects.ToDictionaryAsync(p => p.ProjectName.ToUpper().Trim(), p => p);

                for (int row = 2; row <= rowCount; row++)
                {
                    // Поиск проекта по столбцу O (15-я колонка, "Наименование")
                    var projectNameCell = worksheet.Cells[row, 15].Text?.Trim();
                    if (string.IsNullOrEmpty(projectNameCell))
                    {
                        continue; // Пропускаем строки без названия проекта
                    }

                    if (!projects.TryGetValue(projectNameCell.ToUpper(), out var project))
                    {
                        continue; // Пропускаем, если проект не найден
                    }

                    // Обновление дат начала и окончания проектирования РД
                    project.DateStartRD = ParseDateTime(worksheet.Cells[row, 12].Text?.Trim()); // "Год начала проектирования РД" (столбец M)
                    project.DateEndRD = ParseDateTime(worksheet.Cells[row, 14].Text?.Trim());   // "Год окончания проектирования РД" (столбец N)

                    project.DateStartPD = ParseDateTime(worksheet.Cells[row, 1].Text?.Trim());  // "Год начала проектирования ПД" (столбец A)
                    project.DateFirstApproval = ParseDateTime(worksheet.Cells[row, 3].Text?.Trim()); // "Год положительного заключения (первичного в ГК ОП)" (столбец C)

                    // Обновление общей и продаваемой площади
                    project.TotalArea = ParseDouble(worksheet.Cells[row, 16].Text?.Trim());    // "Общая площадь" (столбец P)
                    project.SaleableArea = ParseDouble(worksheet.Cells[row, 17].Text?.Trim()); // "Продаваемая площадь" (столбец Q)

                    // Обработка корректировок ПД
                    var pdFirstCorrectionStart = ParseDateTime(worksheet.Cells[row, 4].Text?.Trim());  // "Год начала корректировки №1 ПД" (столбец D)
                    var pdFirstCorrectionApproval = ParseDateTime(worksheet.Cells[row, 7].Text?.Trim()); 
                    var pdSecondCorrectionStart = ParseDateTime(worksheet.Cells[row,8 ].Text?.Trim());  
                    var pdSecondCorrectionApproval = ParseDateTime(worksheet.Cells[row, 11].Text?.Trim()); 
                    var pdThirdCorrectionStart = ParseDateTime(worksheet.Cells[row, 12].Text?.Trim());   
                    var pdThirdCorrectionApproval = ParseDateTime(worksheet.Cells[row, 14].Text?.Trim()); 

                    // Добавление корректировок в таблицу ProjectCorrections
                    var corrections = new List<ProjectCorrection>();
                    if (pdFirstCorrectionStart.HasValue)
                    {
                        corrections.Add(new ProjectCorrection
                        {
                            ProjectId = project.Id,
                            CorrectionNumber = 1,
                            StartDate = pdFirstCorrectionStart.Value,
                            ApprovalDate = pdFirstCorrectionApproval
                        });
                    }
                    if (pdSecondCorrectionStart.HasValue)
                    {
                        corrections.Add(new ProjectCorrection
                        {
                            ProjectId = project.Id,
                            CorrectionNumber = 2,
                            StartDate = pdSecondCorrectionStart.Value,
                            ApprovalDate = pdSecondCorrectionApproval
                        });
                    }
                    if (pdThirdCorrectionStart.HasValue)
                    {
                        corrections.Add(new ProjectCorrection
                        {
                            ProjectId = project.Id,
                            CorrectionNumber = 3,
                            StartDate = pdThirdCorrectionStart.Value,
                            ApprovalDate = pdThirdCorrectionApproval
                        });
                    }

                    if (corrections.Any())
                    {
                        await _context.ProjectCorrections.AddRangeAsync(corrections);
                    }

                    // Обновление проекта
                    _context.Projects.Update(project);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Ошибка при импорте данных площадей и лет. Транзакция откатана.", ex);
            }
        }

        private async Task<int> FindBKP(string bkpName)
        {
            if (string.IsNullOrEmpty(bkpName))
            {
                throw new NullReferenceException($"Не удалось найти БКП при импорте данных: {bkpName}");
            }

            var bkp = await _context.BKPs.FirstOrDefaultAsync(x => x.Name == bkpName);
            if (bkp == null)
            {
                bkp = new BKP { Name = bkpName };
                await _context.BKPs.AddAsync(bkp);
                await _context.SaveChangesAsync();
            }

            return bkp.Id;
        }

        private async Task<int> GetOrCreateUserId(string value)
        {
            const int emptyId = 4; // ID записи "empty"
            const int notFoundId = 5; // ID записи "not found"

            if (string.IsNullOrEmpty(value) || value.Length <= 2)
            {
                return emptyId;
            }

            if (string.Equals("Ковалев", value, StringComparison.OrdinalIgnoreCase))
            {
                return 3;
            }

            var sotrudniks = _allSotrudniks
                .Where(x =>
                {
                    var split = x.commonName?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (split?.Length >= 1)
                    {
                        var lastName = split[0];
                        var valueLastName = value.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];
                        return string.Equals(lastName, valueLastName, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                })
                .ToList();

            if (sotrudniks.Count != 1)
            {
                return notFoundId;
            }

            var sotrudnik = sotrudniks.First();
            var dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Login == sotrudnik.login);
            if (dbUser != null)
            {
                return dbUser.Id;
            }

            var user = new User
            {
                Email = sotrudnik.email ?? string.Empty,
                Login = sotrudnik.login ?? throw new InvalidOperationException("LDAP-сотрудник не имеет логина."),
                FIO = sotrudnik.commonName ?? value
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        private double? ParseDouble(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "-" || value.ToLower() == "стоп")
            {
                return null;
            }

            // Очищаем строку от пробелов (включая неразрывный пробел)
            value = value.Trim().Replace(" ", "").Replace("\u00A0", "");

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            return null;
        }

        private DateTime? ParseDateTime(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr) ||
                string.Equals(dateStr, "не получено", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dateStr, "нет", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dateStr, "-", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dateStr, "стоп", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Проверяем, является ли строка просто годом (4 цифры)
            if (int.TryParse(dateStr, out int year) && dateStr.Length == 4 && year >= 1900 && year <= 9999)
            {
                // Если это год, создаем дату 1 января этого года
                return new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            // Если это не просто год, пытаемся распарсить как полную дату
            if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToUniversalTime();
            }

            return null;
        }
    }
}