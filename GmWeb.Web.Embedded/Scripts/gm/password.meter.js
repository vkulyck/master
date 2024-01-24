/*
 * This code is based on a sample from the following website: https://codepen.io/vsync/pen/frudD
*/

function ConfigurePasswordMeter(config, pwd_options) {
    window.pws_config = config;
    pwd_options = pwd_options || {};
    pwd_options.config = config;
    pwd_options.ui = {
        progressBarEmptyPercentage: 14,
        progressBarMinPercentage: 14,
        showErrors: false,
        showPopover: false,
        container: "#" + config.container_id,
        showVerdictsInsideProgressBar: true,
        viewports: {
            progress: config.progress_display_selector,
            errors: config.error_display_selector
        },
        errorMessages: {
            password_too_short: "The Password is too short",
            email_as_password: "Do not use your email as your password",
            same_as_username: "Your password cannot contain your username",
            two_character_classes: "Use different character classes",
            repeated_character: "Too many repetitions",
            sequence_found: "Your password contains sequences",
            password_too_long: "Your password is too long; the maximum length is 32 characters."
        },
        spanError: function (options, key) {
            'use strict';
            var text = options.i18n.t(key);
            if (!text) {
                text = options.ui.errorMessages[key];
            }
            if (!text) {
                return '';
            }
            return '<span style="color: #d52929">' + text + '</span>';
        },
        showErrorTooltip: function (options) {
            var errors = options.instances.errors,
                errorsTitle = options.i18n.t("errorList"),
                message = "<div>" + errorsTitle + "<ul class='error-list' style='margin-bottom: 0;'>";

            $.each(errors, function (idx, err) {
                message += "<li>" + err + "</li>";
            });
            message += "</ul></div>";
            console.log('created error popover:', message);

            var error_tooltip = $(options.config.error_tooltip_selector);
            if (errors.length > 0) {
                //var prev = error_tooltip.data('bs.tooltip').tip().html();
                var error_display = $(options.config.error_display_selector);
                var prev = error_display.html();
                var left = $(prev).html();
                var right = $(message).html();
                console.log('current message:', right);
                console.log('prev message:', left);
                console.log('equal?', left == right);
                if (left == right) {
                    console.log('tooltip is up to date, exiting.');
                }
                else {
                    error_tooltip.tooltip('show');
                    error_display = $(options.config.error_display_selector);
                    console.log('using error display:', error_display);
                    error_display.html(message);
                }
            } else {
                error_tooltip.tooltip('hide');
            }
            return message;
        }
    };
    pwd_options.common = {
        minChar: 8,
        zxcvbn: false,
        onKeyUp: function (evt, data) {
            var options = $('#' + config.editor_id).data("pwstrength-bootstrap");
            var tooltipAnchor = $('#' + config.container_id);
            var errorContainer = $(config.error_display_selector);
            console.log('onKeyUp called, showing tooltip at:', '#' + config.container_id);
            console.log('calling function now with options:', options);
            options.ui.showErrorTooltip(options);
            console.log('evt:', evt);
            console.log('data:', data);
        },
        onScore: function (options, word, score) {
            console.log('onScore called');
            console.log('options:', options);
            console.log('word:', word);
            console.log('score:', score);
            return score;
        }
    };
    pwd_options.rules = {
        activated: {
            maxLength: true
        }
    };
    var editor = $("#" + config.editor_id);
    editor.pwstrength(pwd_options);

    editor.pwstrength("addRule", "maxLength", function (options, word, score) {
        if (word.length >= 32) {
            options.instances.errors.push(options.ui.spanError(options, "password_too_long"));
            return score;
        }
        return false;
    }, -100, true);

    var password_change_handler = function () {
        if ($(this).val() == '') {
            $(config.progress_display_selector).hide();
            $(config.text_display_selector).hide();
        }
        else {
            $(config.progress_display_selector).show();
            $(config.text_display_selector).show();
        }
    };
    editor.on('input propertychange paste', password_change_handler);
    password_change_handler.call(editor.get());
}