
//<script src="~/lib/jquery/jquery.min.js"></script>
//<script src="../vendor/popperjs/popper.min.js"></script>
//<script src="~/lib/bootstrap/js/bootstrap.min.js"></script>
//let button = document.querySelector("#button");
//button.addEventListener("btn btn-primary", (e) => {
//    let log = document.querySelector("#log");
//    switch (e.button) {
//        case 0:
//            log.textContent = "btn-success";
//            break;
//        case 1:
//            log.textContent = "btn-danger";
//            break;

//        default:
//            log.textContent = `Unknown button code: ${e.button}`;
//    }
//});


function clickUnclick(id) {
    const p1 = document.getElementById(id);
    if(p1.style.display == "btn-success"){
        p1.style.display = "btn-danger";
    }else{
        p1.style.display = "btn-success";

    }
}