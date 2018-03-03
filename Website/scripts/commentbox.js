$(document).ready(function () {
    $(".submit-button").on("click", function () {
        var comment = $(".comment-txt-box").val();
        if (comment == "") {
            $(".error-message").show();
        }
        else {
            $(".error-message").hide();
            $(".submit-btn-hidden").click();
        }
    });

    $(".close").on("click", function() {
        $(".overlay").hide();
    }); 
});

