$(document).ready(function () {
    $(".submit-button").on("click", function () {
        $(".previous-comment").val($(".prev-comment-val").html());
        $(".total-warnings").val($(".total-warnings-val").html());
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

    $(".submit-override").on("click", function() {
        $(".submit-shitty-comment").val("true");
        $(".submit-button").click();
    });
});

