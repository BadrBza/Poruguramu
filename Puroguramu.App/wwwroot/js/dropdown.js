
document.addEventListener('DOMContentLoaded', function () {
    const profileButton = document.getElementById('profileButton');
    const dropdown = document.getElementById('dropdown');

    profileButton.addEventListener('click', function () {
        dropdown.classList.toggle('hidden');
    });

    document.addEventListener('click', function (event) {
        if (!profileButton.contains(event.target) && !dropdown.contains(event.target)) {
            dropdown.classList.add('hidden');
        }
    });
});
