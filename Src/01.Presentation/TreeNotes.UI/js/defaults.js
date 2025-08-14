// defaults.js
window._env_ = {
  TREE_NOTES_SERVICE_URL: "REPLACE_TREE_NOTES_SERVICE_URL"
};

const API_BASE_URL = (window._env_.TREE_NOTES_SERVICE_URL.indexOf("_TREE_NOTES_SERVICE_URL") >= 0 ?  window._env_.TREE_NOTES_SERVICE_URL 
                                                                                               : "http://localhost:5100");

toastr.options = {
  "closeButton": true,
  "debug": false,
  "newestOnTop": false,
  "progressBar": true,
  "positionClass": "toast-bottom-right",
  "preventDuplicates": false,
  "onclick": null,
  "showDuration": "300",
  "hideDuration": "1000",
  "timeOut": "3000",
  "extendedTimeOut": "1000",
  "showEasing": "swing",
  "hideEasing": "linear",
  "showMethod": "fadeIn",
  "hideMethod": "fadeOut"
}