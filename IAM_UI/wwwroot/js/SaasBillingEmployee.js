


// For print button Method --------------------------------------------------------
$('#_saasPrint').on('click', function () {
    printReport();
});

// Function to send the formatted data to the server for generating the report
function printReport() {
    // Show the loader
    $('#loader').show();
    $.ajax({
        url: "/SaasBillingEmp/PrintEmpInfo",
        type: "POST",
        success: function (data) {
            if (data.message === "success") {
                const pdfData = data.pdf; // The base64 encoded PDF data
                const byteCharacters = atob(pdfData); // Decode the base64 string into byte array
                const byteArray = new Uint8Array(byteCharacters.length);

                for (let i = 0; i < byteCharacters.length; i++) {
                    byteArray[i] = byteCharacters.charCodeAt(i);
                }

                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = window.URL.createObjectURL(blob);

                // Open the new tab with a fallback download link
                const newTab = window.open();
                $('#loader').hide();
                if (newTab) {
                    newTab.document.write(`
                            <html>
                                <head><title>PDF Report</title></head>
                                <body>
                                    <h1>PDF Report</h1>
                                    <p>If the PDF does not open automatically, click the link below to download it:</p>
                                    <a href="${url}" download="Report.pdf">Download PDF</a>
                                    <script>
                                        window.location.href = "${url}";
                                        setTimeout(() => window.URL.revokeObjectURL("${url}"), 10000);
                                    </script>
                                </body>
                            </html>
                        `);
                } else {
                    $('#loader').hide();
                    alert("Please allow pop-ups for this website to view the PDF.");
                }
            } else {
                $('#loader').hide();
                Swal.fire({
                    icon: 'warning',
                    title: 'No Data Found',
                    text: 'No Information is available in SAAS BILLING'
                });
            }
        },
        error: function (xhr, status, error) {
            $('#loader').hide();
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'An error occurred while generating the report. Please try again.'
            });
        }
    });
}