(function($) {

	"use strict";

	//var options = {
	//    events_source: '/api/content/calendar/events', //  'assets/calendar/events.json',  ////  api/calendar/events
	//	view: 'month',
	//	tmpl_path: 'assets/calendar/tmpls/',
	//	tmpl_cache: false,
	//	day: 'now', //getTodaysDate(), // '2013-03-12',
	//	onAfterEventsLoad: function(events) {
	//		if(!events) {
	//			return;
	//		}
	//		var list = $('#eventlist');
	//		list.html('');

	//		$.each(events, function(key, val) {
	//			$(document.createElement('li'))
	//				.html('<a href="' + val.url + '">' + val.title + '</a>')
	//				.appendTo(list);
	//		});
	//	},
	//	onAfterViewLoad: function(view) {
	//	    $('#calendarTitle').text(this.getTitle());
	//		$('.btn-group button').removeClass('active');
	//		$('button[data-calendar-view="' + view + '"]').addClass('active');
	//	},
	//	classes: {
	//		months: {
	//			general: 'label'
	//		}
	//	}
	//};

	var calendar;
	$(document).ready(function () {
	    calendar = $('#calendar').calendar(options);
	});
	
	function getTodaysDate() {
	    var today = new Date();
	    var dd = today.getDate();
	    var mm = today.getMonth() + 1; //January is 0!
	    var yyyy = today.getFullYear();

	    if (dd < 10) {
	        dd = '0' + dd
	    }

	    if (mm < 10) {
	        mm = '0' + mm
	    }

	    return yyyy + '-' + mm + '-' + dd ;
	}

	$('.calendarView li[data-calendar-view]').each(function () {
	    var $this = $(this);
	    $this.click(function () {
	        calendar.view($this.data('calendar-view'));
	    });
	});


	$('.btn-group button[data-calendar-nav]').each(function() {
		var $this = $(this);
		$this.click(function() {
			calendar.navigate($this.data('calendar-nav'));
		});
	});

	$('.btn-group button[data-calendar-view]').each(function() {
		var $this = $(this);
		$this.click(function() {
			calendar.view($this.data('calendar-view'));
		});
	});

	$('#first_day').change(function(){
		var value = $(this).val();
		value = value.length ? parseInt(value) : null;
		calendar.setOptions({first_day: value});
		calendar.view();
	});

	$('#language').change(function(){
		calendar.setLanguage($(this).val());
		calendar.view();
	});

	$('#events-in-modal').change(function(){
		var val = $(this).is(':checked') ? $(this).val() : null;
		calendar.setOptions({modal: val});
	});
	$('#format-12-hours').change(function(){
		var val = $(this).is(':checked') ? true : false;
		calendar.setOptions({format12: val});
		calendar.view();
	});
	$('#show_wbn').change(function(){
		var val = $(this).is(':checked') ? true : false;
		calendar.setOptions({display_week_numbers: val});
		calendar.view();
	});
	$('#show_wb').change(function(){
		var val = $(this).is(':checked') ? true : false;
		calendar.setOptions({weekbox: val});
		calendar.view();
	});
	//$('#events-modal .modal-header, #events-modal .modal-footer').click(function(e){
	//	//e.preventDefault();
	//	//e.stopPropagation();
    //});

	
}(jQuery));