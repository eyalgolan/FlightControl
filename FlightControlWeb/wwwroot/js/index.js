let map;
let flights = [];
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

    // polling data every 5 seconds.
    setInterval(getFlightsData, 1000);
})

// initalizing map.
function initMap() {
    // creating the map.
    map = L.map('map', {
        center: [20.0, 5.0],
        minZoom: 2,
        zoom: 2
    });

    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(map);

    // when clicking anywhere on the map clear everything to do with the selected flight.
    $("#map").on("click", function (e) {
        if (!e.target.classList.contains('leaflet-marker-icon')) {
            unselectAllMarkers();
            deletePolyLine();
            deleteFromFlightDetails(currSelectedID);
        };
    });
}


// the function create markers on the map based on the data of the flights we polled.
function createMarkers() {
    // creating the markers.
    markersLayer = L.featureGroup();
    map.addLayer(markersLayer);

    let longtitude;
    let latitude;
    let currMarker;
    let markerID;
    let markerToPaint;
    for (let i = 0; i < flights.length; ++i) {
        longtitude = flights[i].flightPlan.initialLocation.longtitude;
        latitude = flights[i].flightPlan.initialLocation.latitude;
        markerID = flights[i].flightID;

        currMarker = L.marker([latitude, longtitude], { id: markerID }).
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

            // TODO: GET requesnt /api/FlightPlan/{id} (instead of the loop)
            let f;
            for (let i = 0; i < flights.length; ++i) {
                if (e.target.options.id === flights[i].flightID) {
                    f = flights[i];
                    break;
                }
            }

            // draws the polyline of the selected flight.
            createPolyline(f.flightPlan);

            // render the flight details table.
            renderFlightDetails(f);
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
    let unselected = L.icon({
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
    let selectedIcon = L.icon({
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

// the function deletes a row from the flight table.
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
    if (elem.childNodes.length) {
        let flightDetailsTH = document.getElementById("flight_details").rows[1].cells[0];
        let detailsId = flightDetailsTH.innerText;
        if (detailsId === id) {
            currSelectedID = 0;
            $("#flight_details thead").html('');
            $("#flight_details tbody").html('');
        }
    }
}

// the function polls data from the server (receiving all flights).
function getFlightsData() {
    //$.get("api/Flights", function (data, status) {
    //    flights = data;
    //    if (toDeleteMarkers) {
    //        map.removeLayer(markersLayer);
    //        toDeleteMarkers = false;
    //    }
    //    renderFlightData(data);
    //    createMarkers();
    //});

    /*ADD AFTER MERGING WITH SERVER:*/
    let nowUTC = new Date().toISOString();
   /* $.get(`api/flights?relative_to=${nowUTC}`, function (allFlights, status) {
        renderFlightData(allFlights);
    });*/

    $.get(`api/flights?relative_to=${nowUTC}&sync_all`, function (allFlights, status) {
        flights = allFlights;
        if (toDeleteMarkers) {
            map.removeLayer(markersLayer);
            toDeleteMarkers = false;
        }
        renderFlightData(allflights);
        createMarkers();
    });
}

// the function renders the data into the flights table.
function renderFlightData(data) {
    let isMyFlight;
    $("#flights tbody").html('');
    $.each(data, function (index, flight) {
        // create the row of the flight.
        let rowHTML = `
            <tr id="${flight.flightID}">
            <th scope="row">${flight.flightID}</th>
            <td>${flight.flightPlan.companyName}</td>`;

        // if its a my flight.
        if (!flight.isExternal) {
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

        // when a row is clicked we render the flight details table.
        rowHTML$.click(function (e) {
            if (!e.target.classList.contains('deleteIcon')) {
                // getting the flightID we wish to paint
                let indexDelAgent = $(this).closest('tr').index('#flights tr');
                let flightTH = document.getElementById("flights").rows[indexDelAgent].cells[0];
                currSelectedID = flightTH.innerText;
                // painting the row.
                //$(this).addClass('text-info').siblings().removeClass('text-info');

                //renderFlightDetails(flight);
                /*ADD AFTER MERGING WITH SERVER:*/
                $.get(`api/FlightPlan/${flight.flightID}`, function (flightPlan, status) {
                    // painting the row.
                    $(this).addClass('text-info').siblings().removeClass('text-info');
                    renderFlightDetails(flightPlan);
                });
                

                // deletes previous poly line.
                deletePolyLine();

                // draw the new poly line.
                createPolyline(flight.flightPlan);
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
                <th scope="col">Departure Time</th>
                <th scope="col">Arrival Time</th>
                <th scope="col">Passengers</th>
                </tr>`;
    let title$ = $(title);
    $("#flight_details thead").append(title$);
}

// the function renders the details of a flight to the table.
function renderFlightDetails(flightPlan) {

    // clearing previous information.
    $("#flight_details thead").html('');
    $("#flight_details tbody").html('');

    // adding the title to the table.
    addTitle();

    // creating the row of the table and appending it.
    /*CHANGE AFTER MERGING WITH SERVER: [without flight.]*/
    let departureTime = new Date(flightPlan.initialLocation.dateTime).toISOString();
    let arrivalTime = calculateArrivalTime(flight.flightPlan);

    let segLen = flightPlan.segments.length - 1;
    let rowHTML = `<tr>
                   <th scope="row">${flightPlan.flightID}</th>
                   <td>${flightPlan.companyName}</td>` +
                  `<td>(${flightPlan.initialLocation.latitude}, ` +
                  `${flightPlan.initialLocation.longtitude})</td>` +
                  `<td>(${flightPlan.segments[segLen].latitude}, ` +
                  `${flightPlan.segments[segLen].longtitude})</td>
                   <td>${departureTime}</td>
                   <td>${arrivalTime}</td>
                   <td>${flightPlan.passengers}</td>
                   </tr>`;
    let rowHTML$ = $(rowHTML);
    $("#flight_details tbody").append(rowHTML$);
}

// the function returns the arrival time by summing the timespanSeconds of all segments.
function calculateArrivalTime(flightPlan) {
    let len = flightPlan.segments.length;
    let sum = 0;
    for (let i = 0; i < len; ++i) {
        sum += flightPlan.segments[i].timespanSeconds;
    }

    // adding the seconds and converting to the required format (yyyy-MM-ddTHH:mm:ssZ).
    let departureTime = new Date(flightPlan.initialLocation.dateTime);
    departureTime.setSeconds(departureTime.getSeconds() + sum);
    let arrivalTime = departureTime.toISOString();
    return arrivalTime;
}

// the functions creates the polyline of the selected flight by it's segments.
function createPolyline(flightPlan) {
    polylineLayer = L.featureGroup();
    map.addLayer(polylineLayer);

    let polyLine = [
        [flightPlan.initialLocation.latitude, flightPlan.initialLocation.longtitude]
    ];
    for (let i = 0; i < flightPlan.segments.length; i++) {
        polyLine[i + 1] = [flightPlan.segments[i].latitude, flightPlan.segments[i].longtitude];
    }
    L.polyline(polyLine).addTo(polylineLayer);
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
    Dropzone.options.myDropzone = {
        // The name that will be used to transfer the file.
        paramName: "files",
        // allows for cancellation of file upload and remove thumbnail.
        addRemoveLinks: true,
        init: function () {
            myDropzone = this;
            myDropzone.on("success", function (file, response) {
                myDropzone.removeFile(file);
            });
        }
    };
}


