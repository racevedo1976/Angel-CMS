define([], function () {
    return {
        attach: function ($form) {

            var $senderName = $form.find("[name=SenderName]"),
                $senderEmail = $form.find("[name=SenderEmail]"),
                $msgSubject = $form.find("[name=MessageSubject]"),
                $msgBody = $form.find("[name=MessageBody]"),
                $sendButton = $form.find("[name=Send]"),
                $sendSuccess = $form.find("[name=Success]")

            var designMode = $form.data().designMode === "true";
            var sendingOrSent = false;

            function hasValue(value) {
                return value !== undefined && value !== null && value.trim() !== "";
            }

            function validateForm() {
                return hasValue($senderName.val())
                    && hasValue($senderEmail.val())
                    && hasValue($msgSubject.val())
                    && hasValue($msgBody.val());
            }


            function updateFormState() {
                if (!designMode && !sendingOrSent && validateForm()) {
                    $sendButton.removeClass("disabled").removeAttr("disabled").prop("disabled", false);
                }
                else {
                    $sendButton.addClass("disabled").attr("disabled", "disabled").prop("disabled", true);
                }
            }

            $senderName.on("keyup", updateFormState);
            $senderEmail.on("keyup", updateFormState);
            $msgSubject.on("keyup", updateFormState);
            $msgBody.on("keyup", updateFormState);
          
            $sendButton.on("click", function () {
                if (designMode) {
                    return alert("Messages cannot be sent in design mode");
                }

                if (!sendingOrSent && validateForm()) {
                    sendingOrSent = true;
                    updateFormState();

                    $form.form().post()
                        .done(function(){
                            $sendSuccess.show();
                        })
                        .fail(function () {
                            sendingOrSent = false;
                            updateFormState();
                        })
                }
            });

        }
    }
});