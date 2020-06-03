let map;
let currSelectedID = 0;

let markersLayer;
let toDeleteMarkers = false;

let polylineLayer;
let toDeletePoly = false;

// initializing drop-zone.
initDropzone();

// initializing project.
$(document).ready(function () {
    initMap();
    deleteOnClick();

    // polling data every second.
    setInterval(getFlightsData, 1000);
})

// initalizing map.
function initMap() {
    // creating the map.
    map = window.L.map('map', {
        center: [20.0, 5.0],
        minZoom: 2,
        zoom: 2
    });

    window.L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(map);

    // when clicking anywhere on the map clear everything to do with the selected flight.
    $("#map").on("click", function (e) {
        if (!e.target.classList.contains('leaflet-marker-icon')) {
            unselectAllMarkers();
            deletePolyLine();
            deleteFromFlightDetails(currSelectedID);
        }
    });
}

// the function polls data from the server (receiving all flights).
function getFlightsData() {
    let nowUTC = new Date().toISOString();
    nowUTC = nowUTC.split('.')[0] + "Z";
    $.get(`api/flights?relative_to=${nowUTC}&sync_all`, function (allFlights, status) {
        let error = document.getElementById("getFlightsError");

        if (status === "success") {
            error.innerHTML = "";
            if (toDeleteMarkers) {
                map.removeLayer(markersLayer);
                toDeleteMarkers = false;
            }
            renderFlightData(allFlights);
            createMarkers(allFlights);

            // if the selected flight doesn't exist anymore (because it has finished).
            if (!isFlightExisting(allFlights)) {
                deletePolyLine();
                deleteFromFlightDetails(currSelectedID);
            }
        } else {
            displayError(2);
        }
    });
}

// the function renders the data into the flights table.
function renderFlightData(data) {
    let isMyFlight;
    $("#flights tbody").html('');
    $.each(data, function (index, flight) {
        // create the row of the flight.
        let rowHTML = `
            <tr id="${flight.flight_id}">
            <th scope="row">${flight.flight_id}</th>
            <td>${flight.company_name}</td>`;

        // if its a my flight.
        if (!flight.is_external) {
            isMyFlight = true;
            rowHTML += `<td>No</td>
                        <td><i value="Delete" type="button" `+
                `alt="Delete4" class="deleteIcon fa fa-trash"></i></td>`;
        } else {
            // its external flight.
            isMyFlight = false;
            rowHTML += `<td>Yes</td>
                        <td> </td>`;
        }
        rowHTML += `</tr>`;
        let rowHTML$ = $(rowHTML);

        // when a row is clicked we render the flight details table,
        // marker the selected flight, draw the polyline and paint the row.
        rowHTML$.click(function (e) {
            if (!e.target.classList.contains('deleteIcon')) {
                // getting the flightID we wish to paint
                let indexDelAgent = $(this).closest('tr').index('#flights tr');
                let flightTH = document.getElementById("flights").rows[indexDelAgent].cells[0];
                currSelectedID = flightTH.innerText;

                $.get(`api/FlightPlan/${flight.flight_id}`, function (flightPlan, status) {
                    let error = document.getElementById("getFlightPlanError");

                    if (status === "success") {
                        error.innerHTML = "";
                        // painting the row.
                        $(this).addClass('text-info').siblings().removeClass('text-info');
                        renderFlightDetails(flightPlan, flight.flight_id);
                        // deletes previous poly line.
                        deletePolyLine();
                        // draw the new poly line.
                        createPolyline(flightPlan);
                    } else {
                        displayError(3);
                    }
                });
            }
        });

        if (isMyFlight) {
            // if it's my flight add to beginning of table.
            $("#flights tbody").prepend(rowHTML$);
        } else {
            // if it's external flight add to end of table.
            $("#flights tbody").append(rowHTML$);
        }
        // paint the selceted row (each time we get data from server we need to re-paint it).
        paintRow();
    });
}


// the function create markers on the map based on the data of the flights we polled.
function createMarkers(flights) {
    // creating the markers.
    markersLayer = window.L.featureGroup();
    map.addLayer(markersLayer);

    let longitude;
    let latitude;
    let currMarker;
    let markerID;
    let markerToPaint;
    for (let i = 0; i < flights.length; ++i) {
        longitude = flights[i].longitude;
        latitude = flights[i].latitude;
        markerID = flights[i].flight_id;

        currMarker = window.L.marker([latitude, longitude], { id: markerID }).
            addTo(markersLayer);
        toDeleteMarkers = true;

        // when a marker is clicked the icon will change to selected.
        $(currMarker).on("click", function (e) {
            // change the icon to selected.
            selectedIconClick(e);

            currSelectedID = e.target.options.id;
            // painting the row.
            $(`#flights ${currSelectedID}`).addClass('text-info').siblings().removeClass('text-info');

            // deletes previous poly line.
            deletePolyLine();

            $.get(`api/FlightPlan/${currSelectedID}`, function (flightPlan, status) {
                let error = document.getElementById("getFlightPlanError");

                if (status === "success") {
                    error.innerHTML = "";
                    // draws the polyline of the selected flight.
                    createPolyline(flightPlan);
                    // render the flight details table.
                    renderFlightDetails(flightPlan, currSelectedID)
                } else {
                    displayError(3);
                }
            });
        });

        if (markerID === currSelectedID) {
            markerToPaint = currMarker;
        }
    }
    // change the icon of all markers to unselected.
    unselectAllMarkers();

    // change the icon of the selceted marker to selected icon.
    if (markerToPaint !== undefined) {
        selectedIconGet(markerToPaint);
    }
}

// the function change the icon of all markers to unselected.
function unselectAllMarkers() {
    // prepare the unselect icon to be ready to use.
    let myURL = $('script[src$="leaflet.js"]').attr('src').replace('leaflet.js', '');
    let unselected = window.L.icon({
        iconUrl: myURL + '../css/images/airplane2x.png',
        iconRetinaUrl: myURL + '../css/images/airplane.png',
        iconSize: [24, 24],
        iconAnchor: [9, 21],
    });

    // unselecting all markers.
    markersLayer.eachLayer(function (layer) {
        layer.setIcon(unselected);
    });
}

// the function creates the selected airplane icon.
function createSelectedIcon() {
    let myURL = $('script[src$="leaflet.js"]').attr('src').replace('leaflet.js', '');
    let selectedIcon = window.L.icon({
        iconUrl: myURL + '../css/images/airplane.png',
        iconRetinaUrl: myURL + '../css/images/airplane2x.png',
        iconSize: [32, 32],
        iconAnchor: [9, 21],
    });

    return selectedIcon;
}

// the function changes the icon of the marker when it's clicked.
function selectedIconClick(e) {
    // unselect all the markers.
    unselectAllMarkers();

    // change the clicked marker to selected icon.
    let selectedIcon = createSelectedIcon();
    e.target.setIcon(selectedIcon);
}

// the function changes the icon of the marker while it's still selected after GET.
function selectedIconGet(marker) {
    // unselect all the markers.
    unselectAllMarkers();

    // change the clicked marker to selected icon.
    let selectedIcon = createSelectedIcon();
    marker.setIcon(selectedIcon);
}

// the function deletes a flight from the application by its flightID.
function deleteOnClick() {
    $(document).on("click", ".deleteIcon", function () {
        // delete the row from the table.
        let indexDelAgent = $(this).closest('tr').index('#flights tr');
        let flightTH = document.getElementById("flights").rows[indexDelAgent].cells[0];
        let id = flightTH.innerText;
        document.getElementById("flights").deleteRow(indexDelAgent);

        // send a delete request to the server.
        $.ajax({
            type: "delete",
            url: "/api/Flights/" + id
        });

        // delete the polyline in case it shows it on the map.
        if (id === currSelectedID) {
            map.removeLayer(polylineLayer);
            toDeletePoly = false;
        }

        // delete the details table in case it shows the flight we wish to delete.
        deleteFromFlightDetails(id);
    });
}

// the function receives flightID from flight table and deletes it from flight details table.
function deleteFromFlightDetails(id) {
    const elem = document.querySelector("#flight_details tbody");
    if (elem.childNodes.length && currSelectedID !== 0) {
        let flightDetailsTH = document.getElementById("flight_details").rows[1].cells[0];
        let detailsId = flightDetailsTH.innerText;
        if (detailsId === id) {
            currSelectedID = 0;
            $("#flight_details thead").html('');
            $("#flight_details tbody").html('');
        }
    }
}

// the function paints a row by flightID
function paintRow() {
    let rowToPaint = document.getElementById(currSelectedID);
    $(rowToPaint).addClass('text-info').siblings().removeClass('text-info');
}

// the function adds the title to the flight details table.
function addTitle() {
    let title = `<tr>
                <th scope="col">Flight ID</th>
                <th scope="col">Airline</th>
                <th scope="col">Initial Location</th>
                <th scope="col">Destination</th>
                <th scope="col">Departure Universal Time</th>
                <th scope="col">Arrival Universal Time</th>
                <th scope="col">Passengers</th>
                </tr>`;
    let title$ = $(title);
    $("#flight_details thead").append(title$);
}

// the function renders the details of a flight to the table.
function renderFlightDetails(flightPlan, flightID) {
    // clearing previous information.
    $("#flight_details thead").html('');
    $("#flight_details tbody").html('');

    // adding the title to the table.
    addTitle();

    // creating the row of the table and appending it.
    let departureTime = new Date(flightPlan.initial_location.date_time).toISOString();
    departureTime = departureTime.split('.')[0] + "Z";
    let arrivalTime = calculateArrivalTime(flightPlan);
    arrivalTime = arrivalTime.split('.')[0] + "Z";

    let segLen = flightPlan.segments.length - 1;
    let rowHTML = `<tr>
                   <th scope="row">${flightID}</th>
                   <td>${flightPlan.company_name}</td>` +
        `<td>(${flightPlan.initial_location.latitude}, ` +
        `${flightPlan.initial_location.longitude})</td>` +
        `<td>(${flightPlan.segments[segLen].latitude}, ` +
        `${flightPlan.segments[segLen].longitude})</td>
                   <td>${departureTime}</td>
                   <td>${arrivalTime}</td>
                   <td>${flightPlan.passengers}</td>
                   </tr>`;
    let rowHTML$ = $(rowHTML);
    $("#flight_details tbody").append(rowHTML$);
}

// the function returns the arrival time by summing the timespan_seconds of all segments.
function calculateArrivalTime(flightPlan) {
    let len = flightPlan.segments.length;
    let sum = 0;
    for (let i = 0; i < len; ++i) {
        sum += flightPlan.segments[i].timespan_seconds;
    }

    // adding the seconds and converting to the required format (yyyy-MM-ddTHH:mm:ssZ).
    let departureTime = new Date(flightPlan.initial_location.date_time);
    departureTime.setSeconds(departureTime.getSeconds() + sum);
    let arrivalTime = departureTime.toISOString();
    return arrivalTime;
}

// the functions creates the polyline of the selected flight by it's segments.
function createPolyline(flightPlan) {
    polylineLayer = window.L.featureGroup();
    map.addLayer(polylineLayer);

    let polyLine = [
        [flightPlan.initial_location.latitude, flightPlan.initial_location.longitude]
    ];
    for (let i = 0; i < flightPlan.segments.length; i++) {
        polyLine[i + 1] = [flightPlan.segments[i].latitude, flightPlan.segments[i].longitude];
    }
    window.L.polyline(polyLine).addTo(polylineLayer);
    toDeletePoly = true;
}

// the function deletes the polyline.
function deletePolyLine() {
    if (toDeletePoly) {
        map.removeLayer(polylineLayer);
        toDeletePoly = false;
    }
}

// the function initializes the drop-zone.
function initDropzone() {
    let dropZone = document.getElementById("myDropzone");
    Dropzone.options.myDropzone = {
        // The name that will be used to transfer the file.
        paramName: "files",
        // allows for cancellation of file upload and remove thumbnail.
        addRemoveLinks: true,
        init: function () {
            dropZone = this;

            dropZone.on("success", function (file) {
                dropZone.removeFile(file);
                let error = document.getElementById("dropzoneError");
                error.innerHTML = "";
            });
            dropZone.on("error", function (file) {
                dropZone.removeFile(file);
                displayError(1);
            });
        }
    };
}

// the function returns 1 if the currSelectedID exists in the list of the 
// flights we get from the server, otherwise 0.
function isFlightExisting(allFlights) {
    let flag = 0;
    for (let i = 0; i < allFlights.length; i++) {
        if (currSelectedID !== 0 && allFlights[i].flight_id === currSelectedID) {
            flag = 1;
            break;
        }
    }
    return flag;
}

// the functions displays the most recent error on the screen.
function displayError(num) {
    let dropzoneError = document.getElementById("dropzoneError");
    let flightsError = document.getElementById("getFlightsError");
    let flightPlanError = document.getElementById("getFlightPlanError");

    // if the error occoured in the dropzone.
    if (num === 1) {
        dropzoneError.innerHTML = "Please insert JSON file in a valid format.";
        flightsError.innerHTML = "";
        flightPlanError.innerHTML = "";
    } else if (num === 2) {
        // if the error occoured in GET all flights.
        dropzoneError.innerHTML = "";
        flightsError.innerHTML = "An error has occoured when trying to receive all flights from server.";
        flightPlanError.innerHTML = "";
    } else if (num === 3) {
        // if the error occoured in GET the flight plan of the selected flight.
        dropzoneError.innerHTML = "";
        flightsError.innerHTML = "";
        flightPlanError.innerHTML = "An error has occoured when trying to receive a flight plan from server.";
    }
}