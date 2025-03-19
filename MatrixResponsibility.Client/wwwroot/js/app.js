window.addEscapeListener = (dotnetObj) => {
    document.addEventListener('keydown', function handler(e) {
        if (e.key === 'Escape') {
            dotnetObj.invokeMethodAsync('OnEscapePressed');
            document.removeEventListener('keydown', handler); // Удаляем после срабатывания
        }
    });
};

window.addEnterListener = (dotnetObj) => {
    document.addEventListener('keydown', function handler(e) {
        if (e.key === 'Enter') {
            dotnetObj.invokeMethodAsync('OnEnterPressed');
            document.removeEventListener('keydown', handler); // Удаляем после срабатывания
        }
    });
};