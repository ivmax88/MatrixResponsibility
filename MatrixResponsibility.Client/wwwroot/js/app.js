﻿let dotNetHelper = null;

window.initializeDotNetHelper = (helper) => {
    dotNetHelper = helper;
    console.log('DotNetHelper initialized');
};

window.addEventListener('keydown', function (event) {
    if (event.key === 'Escape' && dotNetHelper) {
        dotNetHelper.invokeMethodAsync('OnEscapeKeyPressed');
    }
});

// Функция для установки курсора в конец текста
window.setCursorToEnd = (element) => {
    if (element) {
        element.focus(); // Устанавливаем фокус на элемент
        const valueLength = element.value.length;
        element.setSelectionRange(valueLength, valueLength); // Устанавливаем курсор в конец
    } else {
        console.error('Element is null');
    }
};