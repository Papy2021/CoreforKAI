var slideIndex = 0;
var slides = document.querySelectorAll("#slideshow div");

setInterval(function () {
    slides[slideIndex].style.opacity = "0";
    slideIndex = (slideIndex + 1) % slides.length;
    slides[slideIndex].style.opacity = "1";
}, 12000);












