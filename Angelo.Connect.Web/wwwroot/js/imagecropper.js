$.imageCropper = function (options) {
    var defaults = {
        title: "Edit Image",
        imageId: '',            //documentId from Mylibrary
        driveUrl: '',           // *Require*  drive host, url.
        editImageMode: false,   //set to true if image already cropped (previously) and we are just editing that cropped version
        editImageUrl: ''        //if editImageMode = true, then pass the URL used for the cropped image with coordinates.
    };

    var cropper;
    var imageElement;
    var cropBoxData;
    var canvasData;

    // Init
    options = $.extend({}, defaults, options);

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    var template = [
            '<div class="media-cropper modal-dialog" style="width:80%; height:auto;">',
              '<div class="modal-content modal-layout">',
                '<div class="media-cropper-heading">',
                    '<span class="media-cropper-title"></span>',
                '</div>',
                '<div class="row media-cropper-body">',
                     '<div class="img-container">',
                        '<img style="max-width: 100%" id="media-cropper-image" src="' + options.driveUrl + '/download/' + options.imageId + '" alt="Picture">',
                     '</div>',
                 '</div>',
                '<div class="media-cropper-footer">',
                    '<div class="clearfix">',
                        '<div class="pull-left">',
                            '<select name="media-cropper-aspectratio" class="form-control" id="media-cropper-aspectratio">',
                                '<option value="NaN">Free form</option>',
                                '<option value="1.7777777777777777">16:9</option>',
                                '<option value="1.3333333333333333">4:3</option>',
                                '<option value="1">1:1</option>',
                                '<option value="0.6666666666666666">2:3</option>',
                            '</select>',
                       ' </div>',
                        '<div class="media-cropper-commands pull-right">',
                            '<button name="select" class="btn btn-default">&nbsp; Apply &nbsp;</button>',
                            '<button name="cancel" class="btn btn-default">Cancel</button>',
                        '</div>',
                    '</div>',
                '</div>',
             '</div>',
            '</div>'
    ];
    var $modal = $('<div class="modal modal-layout" style="z-index:1080"></div>');
    var task = $.Deferred();
    var $backdrop = $('<div class="modal-backdrop fade show"></div>');

    $backdrop
       .css({
           display: "block",
           opacity: 0.5,
           "z-index": 1075
       })
       .prependTo(document.body);

    $modal
       .css({ display: "block" })
       .append(template.join(''))
       .prependTo(document.body)
       .find(".media-cropper-title")
       .html(options.title);


    // Init Buttons
    $modal.find(".media-cropper-commands [name=select]")
        .on("click", function (event) {
            var url = {
                id: options.imageId
            };
            if (cropper) {

                if (!options.imageId)
                    return;

                var imageData = cropper.getData({
                    rounded: true
                });

                var aspectRatio = $("#media-cropper-aspectratio").val();

                url.croppedUrl = options.driveUrl + '/download/crop?id=' + encodeURIComponent(options.imageId) + '&x=' + imageData.x + '&y=' + imageData.y + '&width=' + imageData.width + '&height=' + imageData.height + '&aratio=' + aspectRatio, '_blank';
            }

            var result = task.resolve(url);

            $.isPromise(result)
                ? result.done(function () {
                    $modal.remove();
                    $backdrop.remove();
                })
                : $modal.remove();
        });

    $modal.find(".media-cropper-commands [name=cancel]")
        .on("click", function (event) {
            // just close the modal w/o resolving the task
            $modal.remove();
            $backdrop.remove();
        });

    $modal.find("#media-cropper-aspectratio").on("change", function() {
        var selection = $(this).val();
        
        if (cropper) {
            cropper.setAspectRatio(selection);
        }
    })
    function initCropper() {
        imageElement = document.getElementById('media-cropper-image');
        var aspectRatio = NaN;

        if (options.editImageMode) {
            options.imageId = getParameterByName("id", options.editImageUrl);
            if (options.imageId === null) {
                //document Id was not found, then try to extract from url based on this format:  "http://drive.com/documents/documentid
                options.imageId = options.editImageUrl.substr(options.editImageUrl.lastIndexOf('/') + 1);
            }
            var left = getParameterByName("x", options.editImageUrl);
            var top = getParameterByName("y", options.editImageUrl);
            var width = getParameterByName("width", options.editImageUrl);
            var height = getParameterByName("height", options.editImageUrl);
            aspectRatio = getParameterByName("aratio", options.editImageUrl);

            if (left !== null && top !== null & width !== null & height !== null) {
                cropBoxData = {
                    left: Number(left),
                    top: Number(top),
                    width: Number(width),
                    height: Number(height)
                }
            }
        }

        imageElement.src = options.driveUrl + '/download/' + options.imageId;
        
        $("#media-cropper-aspectratio").val(aspectRatio ? aspectRatio : NaN);

        if (cropper) {
            cropper.destroy();
            cropper = null;
        }

        cropper = new Cropper(imageElement, {
            autoCropArea: 0.5,
            viewMode: 2,
            checkImageOrigin: false,
            checkCrossOrigin: false,
            aspectRatio: aspectRatio,
            ready: function () {

                var canvasData = cropper.getCanvasData();

                if (cropBoxData) {
                    cropBoxData.width = cropBoxData.width * (canvasData.width / canvasData.naturalWidth);
                    cropBoxData.height = cropBoxData.height * (canvasData.height / canvasData.naturalHeight)

                    //experimental 
                    cropBoxData.left = cropBoxData.left * (canvasData.width / canvasData.naturalWidth);
                    cropBoxData.top = cropBoxData.top * (canvasData.height / canvasData.naturalHeight)
                }

                //Should set crop box data first here
                cropper.setCropBoxData(cropBoxData).setCanvasData(canvasData);

            }
        });

    }

    initCropper();
    // Return promise
    return task.promise();
}
