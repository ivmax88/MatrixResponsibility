using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MatrixResponsibility.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bkps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    director_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bkps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bkps_users_director_id",
                        column: x => x.director_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    gip_id = table.Column<int>(type: "integer", nullable: true),
                    assistant_gip_id = table.Column<int>(type: "integer", nullable: true),
                    gap_id = table.Column<int>(type: "integer", nullable: true),
                    gkp_id = table.Column<int>(type: "integer", nullable: true),
                    ab = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gp_id = table.Column<int>(type: "integer", nullable: true),
                    eom_id = table.Column<int>(type: "integer", nullable: true),
                    ss_id = table.Column<int>(type: "integer", nullable: true),
                    ak_id = table.Column<int>(type: "integer", nullable: true),
                    responsible_id = table.Column<int>(type: "integer", nullable: true),
                    internal_meeting = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    report_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    gpzu_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    customer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    start_permission_letter = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    marketing_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    object_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    bkp_id = table.Column<int>(type: "integer", nullable: true),
                    date_start_pd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_first_approval = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_start_rd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_end_rd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    total_area = table.Column<double>(type: "double precision", nullable: true),
                    saleable_area = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_projects_bkps_bkp_id",
                        column: x => x.bkp_id,
                        principalTable: "bkps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_ak_id",
                        column: x => x.ak_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_assistant_gip_id",
                        column: x => x.assistant_gip_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_eom_id",
                        column: x => x.eom_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_gap_id",
                        column: x => x.gap_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_gip_id",
                        column: x => x.gip_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_gkp_id",
                        column: x => x.gkp_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_gp_id",
                        column: x => x.gp_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_responsible_id",
                        column: x => x.responsible_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_ss_id",
                        column: x => x.ss_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "project_corrections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approval_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    correction_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_corrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_project_corrections_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Администратор", "admin" },
                    { 2, "Директор БКП", "dbkp" },
                    { 3, "Главный инженер проекта", "gip" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "login" },
                values: new object[,]
                {
                    { 1, "ivanov" },
                    { 2, "tsarev" }
                });

            migrationBuilder.InsertData(
                table: "bkps",
                columns: new[] { "Id", "director_id", "name" },
                values: new object[,]
                {
                    { 1, 2, "БКП №1" },
                    { 2, 2, "БКП №2" },
                    { 3, 2, "БКП №3" },
                    { 4, 2, "БКП №4" },
                    { 5, 2, "БКП №5" },
                    { 6, 2, "БКП №6" },
                    { 7, 2, "БКП №7" }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "role_id", "user_id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_bkps_director_id",
                table: "bkps",
                column: "director_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_corrections_project_id",
                table: "project_corrections",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_ak_id",
                table: "projects",
                column: "ak_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_assistant_gip_id",
                table: "projects",
                column: "assistant_gip_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_bkp_id",
                table: "projects",
                column: "bkp_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_eom_id",
                table: "projects",
                column: "eom_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_gap_id",
                table: "projects",
                column: "gap_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_gip_id",
                table: "projects",
                column: "gip_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_gkp_id",
                table: "projects",
                column: "gkp_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_gp_id",
                table: "projects",
                column: "gp_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_responsible_id",
                table: "projects",
                column: "responsible_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_ss_id",
                table: "projects",
                column: "ss_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "project_corrections");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "bkps");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
