document.addEventListener("DOMContentLoaded", function () {
    var categorySelect = document.getElementById('c');
    var sortingSelect = document.getElementById('s');
    var switchElement = document.getElementById('flexSwitchCheckDefault');

    // 恢复保存的选项
    var savedCategory = localStorage.getItem('category');
    var savedSorting = localStorage.getItem('sorting');
    var savedVegetarian = localStorage.getItem('vegetarian');

    if (savedCategory) {
        var categories = savedCategory.split(',');
        for (var i = 0; i < categorySelect.options.length; i++) {
            if (categories.includes(categorySelect.options[i].value)) {
                categorySelect.options[i].selected = true;
            }
        }
    }

    if (savedSorting) {
        sortingSelect.value = savedSorting;
    }

    if (savedVegetarian) {
        switchElement.checked = savedVegetarian === 'true';
    }

    // 保存选择的选项
    document.querySelector('.filterForm').addEventListener('submit', function () {
        var selectedCategories = [...categorySelect.selectedOptions].map(option => option.value);
        try {
            localStorage.setItem('category', selectedCategories.join(','));
            localStorage.setItem('sorting', sortingSelect.value);
            localStorage.setItem('vegetarian', switchElement.checked);
            console.log('success ');
        }
        catch {
            console.error('Error storing item: ', e);
        }
    });

    // 延迟触发 change 事件
    setTimeout(function () {
        var event = new Event('change', { bubbles: true });
        categorySelect.dispatchEvent(event);
        sortingSelect.dispatchEvent(event);
        switchElement.dispatchEvent(event);
    }, 100); // 可以根据实际情况调整延迟时间
});
