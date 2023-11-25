function loadFiltersFromLocalStorage() {
    const selectedCategory = localStorage.getItem('category');
    const selectedSorting = localStorage.getItem('sorting');
    const showVegan = localStorage.getItem('vegetarian');

    var categorySelect = document.getElementById('c');
    var sortingSelect = document.getElementById('s');
    var switchElement = document.getElementById('flexSwitchCheckDefault');


    if (selectedCategory) {

        var categories = selectedCategory.split(',');
        const categorySelect = document.getElementById('c');
        for (var i = 0; i < categorySelect.options.length; i++) {
            if (categories.includes(categorySelect.options[i].value)) {
                categorySelect.options[i].selected = true;
            }
        }

        $(categorySelect).selectpicker('refresh');
    }
    if (selectedSorting) {
        var sortingSelect = document.getElementById('s');
        sortingSelect.value = selectedSorting;
        $(sortingSelect).selectpicker('refresh');
    }
    if (showVegan) {
        document.getElementById('flexSwitchCheckDefault').checked = showVegan === 'true';
    }
}


window.onload = function () {
    loadFiltersFromLocalStorage();
};

document.querySelector('.filterForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const selectedCategory = [...document.getElementById('c').options]
        .filter(option => option.selected)
        .map(option => option.value);
    const selectedSorting = document.getElementById('s').value;
    const showVegan = document.getElementById('flexSwitchCheckDefault').checked;

    localStorage.setItem('category', selectedCategory);
    localStorage.setItem('sorting', selectedSorting);
    localStorage.setItem('vegetarian', showVegan);

    this.submit();
});