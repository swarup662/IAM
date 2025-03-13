$(document).ready(function () {

    var modal = new bootstrap.Modal(document.getElementById('companyModal'));
    modal.show();

    $('#companyDropdown').on('change', function () {
        var selectedCompanyId = $(this).val();
        $('#applicationsContainer').empty();

        if (selectedCompanyId) {
            $.ajax({
                url: '/Login/GetApplicationsByCompany',
                type: 'GET',
                data: { companyId: selectedCompanyId },
                success: function (data) {
                    if (data && data.applications) {
                        $('#applicationsContainer').empty();

                        var container = document.getElementById('applicationsContainer');
                        var row;

                        data.applications.forEach(function (app, index) {
                            if (index % 4 === 0) {
                                row = document.createElement("div");
                                row.classList.add("row", "app-row");
                                container.appendChild(row);
                            }
                            app.name = app.name.toLowerCase().replace(/\b\w/g, char => char.toUpperCase());
                            var appCard = document.createElement("div");
                            appCard.classList.add("col-md-3", "d-flex", "justify-content-center", "g-2");
                            appCard.innerHTML = `
                                <a class="card" link="/Login/SetApplication?Key=${app.data}" 
                                    style="--bg-color: ${app.color}; --bg-color-light: #f1f7ff; --text-color-hover: #e4e4e4; --box-shadow-color: rgba(220, 233, 255, 0.48);">
                                    <div class="overlay"></div>
                                    <div class="circle"> <i class="${app.icon}"></i></div>
                                    <p class="name">${app.name}</p>
                                </a>
                            `;

                            row.appendChild(appCard);
                        });

                        $(".card").on("click", function (event) {
                            $('.loader').fadeIn(); // Show loader with fade effect
                            event.preventDefault(); // Prevent immediate redirection
                            var url = $(this).attr("link"); // Get link URL
                            var card = $(this);
                            // Disable further clicks on this card
                            card.off("click"); // Remove click event handler


                            //// Apply animation effect
                            //card.addClass("sparkle-effect")

                            // Delay redirection to match animation
                            //setTimeout(function () {
                              window.location.href = url;
                            //}, 1200); // Redirect after 1.2s (matches animation)
                        });

                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching applications data:', error);
                }
            });
        }
    });
});




