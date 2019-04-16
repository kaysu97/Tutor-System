$(".listIconSvg").click(function () {
    $(".listIconClickBody").addClass("listIconClickBodyOpen");
    $(".listIconClick").addClass("listIconClickOpen");
});
$(".listIconClickBody").click(function () {
    $(".listIconClickBody").removeClass("listIconClickBodyOpen");
    $(".listIconClick").removeClass("listIconClickOpen");
});