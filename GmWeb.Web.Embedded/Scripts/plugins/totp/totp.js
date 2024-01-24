totpjs = {};

// CLICK 'LOAD A LARGE FILE' TO START THE PIE TIMER
$(function () {
    $('.fakeLoad').click(function () {
        $('div#pie_to_be').pietimer({
            seconds: 8,
            colour: 'rgba(200, 150, 225, 1)'
        }, function () {
            $('.done').fadeIn(400).delay(400).fadeOut(400)
        });
    })




    // CLICK 'STOP' TO CLEAR INTERVAL AND HIDE THE PIE TIMER
    $('.stop').click(function () {

        $('#pie_timer').fadeOut(400);
        clearInterval(interval);
        canvas.width = 0;

    });


});

totpjs.utils = (function () {
    var
        vec_argmin = function (vec) {
            var minIdx = NaN, minVal = NaN;
            m.forEach(function (value, index, matrix) {
                if (isNaN(minIdx)) {
                    minIdx = index;
                    minVal = value;
                } else if (value < minVal) {
                    minIdx = index;
                    minVal = value;
                }
            });
            return math.squeeze(minIdx);
        },
        format = function () {
            var s = arguments[0];
            var format_args = totpjs.utils.flatten(arguments.slice(1));
            for (var i = 0; i < format_args.length - 1; i++) {
                var reg = new RegExp("\\{" + i + "\\}", "gm");
                s = s.replace(reg, format_args[i]);
            }
            return s;
        },
        flatten = function (...input) {
            return input.reduce(function (prev, curr) {
                if (Array.isArray(curr)) {
                    return prev.concat(flattenMix(...curr));
                } else {
                    return prev.concat(curr);
                }
            }, []);
        },
        to_degrees = function (rads) {
            return rads * 180.0 / Math.PI
        },
        to_radians = function (degrees) {
            return degrees * Math.PI / 180.0;
        }
    return {
        format: format,
        flatten,
        math: {
            argmin: vec_argmin,
            to_degrees: to_degrees,
            to_radians: to_radians,
        }
    }
})();

totpjs.ui = (function ($) {
    var ColorAnchor = function (options) {
        this.options = options;
        this.settings = {};
        this.alpha = 1.0;
        this.init();
    };
    ColorAnchor.prototype = function () {
        var
            default_settings = {
                rgb: [0x00, 0x00, 0xFF],
                alpha: 1.0,
                anchor: 0.5
            },
            init = function () {
                if (this.options) {
                    $.extend(this.settings, default_settings, this.options);
                } else {
                    $.extend(this.settings, default_settings);
                }

                if (this.settings.rgba !=== null) {
                    this.settings.alpha = this.settings.rgba[3];
                    this.settings.rgb = this.settings.rgba.slice(0, 3);
                    delete this.settings.rgba;
                }
            },

        return {
            init: init
        }
    }();
    var ProgressGradientEffect = function (options) {
        this.options = options;
        this.settings = {
            color_sequence: [
                ColorAnchor({
                    rgb: [0x48, 0x7c, 0xff],
                    anchor: 0.0,
                }),
                ColorAnchor({
                    rgb: [0xff, 0xd0, 0x32],
                    anchor: 40.0,
                }),
                ColorAnchor({
                    rgb: [0xff, 0x32, 0x81],
                    anchor: 100.0,
                }),
            ]
        };
        this.color = NaN;
        this.init();
    };
    ProgressGradientEffect.prototype = function () {
        var
            _get_anchor_values = function () {
                var values = [];
                $.each(this.settings.color_sequence, function (idx, color_anchor) {
                    values.push(color_anchor.anchor);
                });
                return values;
            },
            _get_gradient_matrix = function () {
                var gradients = [];
                $.each(this.settings.color_sequence, function (idx, color_anchor) {
                    gradients.push(color_anchor.rgb);
                });
                return gradients;
            },
            _compute_progress_color = function (progress) {
                const anchors = math.matrix(_get_anchor_values());
                const distances = math.abs(math.subtract(anchors, progress));
                const weights = math.divide(distances, math.sum(distances));
                const min_index = totpjs.utils.math.argmin(weights);
                const trimmed_weights = weights.map(function (value, index, matrix) {
                    if (index == min_index) {
                        return 0.0;
                    }
                    return value;
                });
                console.log('weights:', weights.toArray(), 'min_index:', min_index, 'trimmed_weights:', trimmed_weights.toArray());
                var gradients = math.matrix(_get_gradient_matrix());

                const gradients = math.matrix([
                    [0x48, 0x7c, 0xff],
                    [0xff, 0xd0, 0x32],
                    [0xff, 0x32, 0x81]
                ]).transpose();
                const color = math.multiply(gradients, trimmed_weights);
                return color.toArray();
            },
            update = function (progress, element) {
                this.progress = progress;
                this.color = _compute_progress_color(progress);
                var rgb_string = totpjs.utils.format('rgba({0},{1},{2})', this.color);
                element.css({ color: rgb_string });
            },
            init = function() {
                this.update(0.0);
            };
            return {
                update: update
            };
        }
    var PieTimer = function (options) {
        this.default_settings = {
            ui: {
                effects: [
                    new ProgressGradientEffect()
                ],
                selector: '#pie-timer',
                height: 25,
                width: 25
            },
            proc: {
                cycle_time: moment.duration(30, 'seconds'),
                cycle_count: 3,
                update_period_ms: 25
            }
        };
        this.options = options;
        this.settings = {};
        this.progress = 0.0;
        this.degrees = 360.0;
    };

    PieTimer.prototype = function () {
        var
            init = function () {
                if (this.options) {
                    $.extend(this.settings, default_settings, this.options);
                } else {
                    $.extend(this.settings, default_settings);
                }
                this.element = $(this.settings.ui.selector);
                _init_statistics();
                _build_ui();
            },
            _init_statistics = function () {
                var max_degrees = 360.0, max_percentage = 100.0;
                var total_seconds = this.settings.proc.cycle_time * this.settings.proc.cycle_count;
                var total_ms = total_seconds * 1000;
                this.settings.proc.update_degrees = max_degrees * this.settings.proc.update_period_ms / total_ms;
                this.settings.proc.update_percentage = max_percentage * this.settings.proc.update_period_ms / total_ms;
                this.progress
            },
            _fmt = totpjs.utils.format,
            _build_ui = function () {
                var h = _fmt('<canvas id="pie_timer" width="{0}" height="{1}"></canvas>', this.settings.width, this.settings.height);
                this.html(h);
            },
            start = function () {
                this.interval = setInterval(this.update, this.settings.proc.update_period_ms);
            },
            _update_canvas = function() {
                var canvas = this.element.get();
                if (canvas.getContext) {
                    if (this.degrees <= 0) {
                        clearInterval(this.interval);
                        var callback = this.settings.on_completed;
                        if (typeof callback == 'function') {
                            callback.call();
                        }
                    } else {
                        var ctx = canvas.getContext('2d');
                        var canvas_size = [canvas.width, canvas.height];
                        var radius = Math.min(canvas_size[0], canvas_size[1]) / 2;
                        var center = [canvas_size[0] / 2, canvas_size[1] / 2];

                        ctx.beginPath();
                        ctx.moveTo(center[0], center[1]);
                        var start_degrees = 270.0;
                        var start_radians = totpjs.utils.math.to_radians(start_degrees);
                        ctx.arc(
                            center[0],
                            center[1],
                            radius,
                            start - this.radians,
                            start,
                            false
                        );

                        ctx.closePath();
                        ctx.fillStyle = settings.colour;
                        ctx.fill();
                    }
                }
            },
            update = function () {
                var effects = this.settings.ui.effects;
                for (var i = 0; i < effects.length; i++) {
                    var effect = effects[i];
                    effect.update(this.progress, this.element);
                } if (typeof callback == 'function') {
                    callback.call();
                }
                this.progress += this.settings.proc.update_percentage;
                this.degrees -= this.settings.proc.update_degrees;
                this.radians = totpjs.utils.math.to_radians(this.degrees);
                _update_canvas();
            }
        return {
            init: init, 
            update: update
        }
    }();
})(jQuery);