
(function ($) {
	$.fn.mungResizable = function () {
		var _ = this;

		$('.draggable-handler').mousedown(function (e) {
			drag = $(this).closest('.draggable')
			drag.addClass('dragging')
			drag.css('left', e.clientX - $(this).width() / 2)
			drag.css('top', e.clientY - $(this).height() / 2 - 10)
			$(this).on('mousemove', function (e) {
				drag.css('left', e.clientX - $(this).width() / 2)
				drag.css('top', e.clientY - $(this).height() / 2 - 10)
				window.getSelection().removeAllRanges()
			})
		})

		$('.draggable-handler').mouseleave(stopDragging)
		$('.draggable-handler').mouseup(stopDragging)

		function stopDragging() {
			drag = $(this).closest('.draggable')
			drag.removeClass('dragging')
			$(this).off('mousemove')
		}


	}

}(jQuery));


(function ($) {
	$.fn.jqDrag = function (h) { return i(this, h, 'd'); };
	$.fn.jqResize = function (h) { return i(this, h, 'r'); };
	$.jqDnR = {
		dnr: {}, e: 0,
		drag: function (v) {
			if (M.k == 'd') E.css({ left: M.X + v.pageX - M.pX, top: M.Y + v.pageY - M.pY });
			else E.css({
				width: Math.max(v.pageX - M.pX + M.W, 0),
				height: Math.max(v.pageY - M.pY + M.H, 0)
			});
			return false;
		},
		stop: function () { E.css('opacity', M.o); $().unbind('mousemove', J.drag).unbind('mouseup', J.stop); }
	};
	var J = $.jqDnR, M = J.dnr, E = J.e,
	i = function (e, h, k) {
		return e.each(function () {
			h = (h) ? $(h, e) : e;
			h.bind('mousedown', { e: e, k: k }, function (v) {
				var d = v.data, p = {}; E = d.e;
				// attempt utilization of dimensions plugin to fix IE issues
				if (E.css('position') != 'relative') { try { E.position(p); } catch (e) { } }
				M = { X: p.left || f('left') || 0, Y: p.top || f('top') || 0, W: f('width') || E[0].scrollWidth || 0, H: f('height') || E[0].scrollHeight || 0, pX: v.pageX, pY: v.pageY, k: d.k, o: E.css('opacity') };
				E.css({ opacity: 0.8 }); $().mousemove($.jqDnR.drag).mouseup($.jqDnR.stop);
				return false;
			});
		});
	},
	f = function (k) { return parseInt(E.css(k)) || false; };
})(jQuery);