
(function ($) {
    "use strict";

    function CalendarApp() {
        this.$body = $("body");
        this.$modal = $("#event-modal");
        this.$calendar = $("#calendar");
        this.$formEvent = $("#form-event");
        this.$btnNewEvent = $("#btn-new-event");
        this.$btnDeleteEvent = $("#btn-delete-event");
        this.$btnSaveEvent = $("#btn-save-event");
        this.$modalTitle = $("#modal-title");
        this.$calendarObj = null;
        this.$selectedEvent = null;
        this.$newEventData = null;
    }

    CalendarApp.prototype.onEventClick = function (e) {
        this.$formEvent[0].reset();
        this.$formEvent.removeClass("was-validated");
        this.$newEventData = null;
        this.$btnDeleteEvent.show();
        this.$modalTitle.text("Edit Holidays");
        this.$modal.show();
        this.$selectedEvent = e.event;
        console.log(e);
        $("#event-title").val(this.$selectedEvent.title);
        $("#event-category").val(this.$selectedEvent.classNames[0]);
    };

    CalendarApp.prototype.onSelect = function (e) {
        this.$formEvent[0].reset();
        this.$formEvent.removeClass("was-validated");
        this.$selectedEvent = null;
        this.$newEventData = e; 
        this.$btnDeleteEvent.hide();
        this.$modalTitle.text("Add Holidays");
        this.$modal.show();
        this.$calendarObj.unselect();
        $("#holiday_Date").val(e.dateStr);
    };

    CalendarApp.prototype.init = function () {
        this.$modal = new bootstrap.Modal(document.getElementById("event-modal"), { keyboard: false });
        var today = new Date($.now());

       

        var events = [
            {
                title: "Meeting with Mr. Shreyu",
                start: new Date($.now() + 158e6),
                end: new Date($.now() + 338e6),
                className: "bg-warning"
            },
            {
                title: "Interview - Backend Engineer",
                start: today,
                end: today,
                className: "bg-success"
            },
            {
                title: "Phone Screen - Frontend Engineer",
                start: new Date($.now() + 168e6),
                className: "bg-info"
            },
            {
                title: "Buy Design Assets",
                start: new Date($.now() + 338e6),
                end: new Date($.now() + 4056e5),
                className: "bg-primary"
            }
        ];

        var self = this;

        self.$calendarObj = new FullCalendar.Calendar(self.$calendar[0], {
            slotDuration: "00:15:00",
            slotMinTime: "08:00:00",
            slotMaxTime: "19:00:00",
            themeSystem: "bootstrap",
            bootstrapFontAwesome: false,
            buttonText: {
                today: "Today",
                month: "Month",
                week: "Week",
                day: "Day",
                list: "List",
                prev: "Prev",
                next: "Next"
            },
            initialView: "dayGridMonth",
            handleWindowResize: true,
            height: $(window).height() - 200,
            headerToolbar: {
                left: "prev,next today",
                center: "title",
                right: "dayGridMonth,timeGridWeek,timeGridDay,listMonth"
            },
            initialEvents: events,
            editable: true,
            droppable: true,
            selectable: true,
            dateClick: function (event) {
                self.onSelect(event);
            },
            eventClick: function (event) {
                self.onEventClick(event);
            }
        });

        self.$calendarObj.render();

        self.$btnNewEvent.on("click", function (event) {
            self.onSelect({ date: new Date(), allDay: true });
        });

        self.$formEvent.on("submit", function (event) {
            event.preventDefault();
            var formElement = self.$formEvent[0];
            if (formElement.checkValidity()) {
                if (self.$selectedEvent) {
                    // Update existing event
                    var eventData = {
                        holiday_Date: $("#holiday_Date").val(),
                        stateId: $("#stateId").val(),
                        holiday_Name: $("#holiday_Name").val(),
                        holiday_Desc: $("#holiday_Desc").val(),
                    };
                    // Use the saveEvent function to save the event
                    //saveEvent(eventData);
                    self.$selectedEvent.setProp("holiday_Name", eventData.holiday_Name);
                    // You may need to update other event properties as well
                    self.$modal.hide();
                } else {
                    // Create a new event
                    var newEventData = {
                        date: $("#holiday_Date").val(),
                        stateId: $("#stateId").val(),
                        title: $("#holiday_Name").val(),
                        holiday_Desc: $("#holiday_Desc").val(),
                        allDay: true, // Assuming this is an all-day event
                        className: "bg-danger", // Set the desired class
                        // You can set additional properties here
                    };
                    // Use the saveEvent function to save the event
                     
                    //saveEvent(newEventData);
                    self.$calendarObj.addEvent(newEventData); 
                    self.$modal.hide();
                }
                
            } else {
                event.stopPropagation();
                formElement.classList.add("was-validated");
            }
        });

        self.$btnDeleteEvent.on("click", function (event) {
            if (self.$selectedEvent) {
                self.$selectedEvent.remove();
                self.$selectedEvent = null;
                self.$modal.hide();
            }
        });
    };

    $.CalendarApp = new CalendarApp();
    $.CalendarApp.Constructor = CalendarApp;

})(window.jQuery);

(function () {
    "use strict";
    window.jQuery.CalendarApp.init();
})();
