import { darkModeStyles } from './mapsTheme.js'

var map
var marker
var newMarker
var markers = []
var infowindow
var time
let historyClicked = false
var polyline
var polylines = []
export function initMap (latData = 41.1082, lngData = 28.9784) {
    time = moment().format('YYYY-MM-DD HH:mm:ss')
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: latData, lng: lngData },
        zoom: 14,
        styles: darkModeStyles
    })
    marker = new google.maps.Marker({
        position: { lat: latData, lng: lngData },
        map: map,
        icon: {
            path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
            scale: 4,
            rotation: 0,
        }, // googlenin "gezi" simgesi
        title: 'Vehicle',
        customTime: time
    })
    infowindow = new google.maps.InfoWindow()
    marker.addListener('click', function () {
        var konum = marker.getPosition()
        infowindow.setContent('Konum: ' + konum.toUrlValue(6) + '<br> Zaman: ' + time)
        infowindow.open(map, marker)
    })
    polyline = new google.maps.Polyline({
        path: polylines,
        geodesic: true,
        strokeColor: '#0000FF',
        strokeOpacity: 1.0,
        strokeWeight: 2
    })
}

export function loadScript () {
    fetch('/Home/GetMapsApiKey').then(response => response.json())
        .then(data => {
            var script = document.createElement('script')
            script.type = 'text/javascript'
            script.src = 'https://maps.googleapis.com/maps/api/js?key=' + data.apiKey + '&callback=initMap&loading=async'
            document.body.appendChild(script)
        })
        .catch(error => {
            console.error('Error fetching Maps API Key:', error)
        })
}

export function updateMap (location) { //update maps
    if (historyClicked) {
        loadNewHistoryLocation()
    }
    let icon = marker.getIcon()
    // Rotasyonu ayarla
    icon.rotation = angleMarker(location, marker.getPosition())
    // Yeni ikonu marker'a set et
    marker.setIcon(icon)

    time = moment(location.timestamp).format('YYYY-MM-DD HH:mm:ss')
    var newLatLng = new google.maps.LatLng(location.latitude, location.longitude)
    marker.setPosition(newLatLng)
    map.panTo(newLatLng)
    marker.customTime = time

    var contentString = 'Konum: ' + newLatLng.toUrlValue(6) + '<br> Zaman: ' + time
    infowindow.setContent(contentString)
    if (infowindow.getMap()) {
        infowindow.open(map, marker)
    }
    // Polyline'a marker konumunu ekle
    polylines.push(marker.getPosition())
    polyline.setPath(polylines)
}

export function loadHistoryLocations () {
    $(document).on('click', '#history', function () {
        if (historyClicked == false) {
            historyClicked = true
            $.ajax({
                type: 'GET', // HTTP method
                url: 'http://localhost:5210/Home/GetLocationHistory', // MVC Action Method URL
                dataType: 'json', // Data type expected from the server
                success: function (response) {
                    // Handle successful response
                    polylines = []
                    $.each(response, function (index, item) {
                        var time = item.timestamp
                        var newLatLng = new google.maps.LatLng(item.latitude, item.longitude)

                        createNewMarker(newLatLng, time)

                        // Polyline'a konumu ekle
                        polylines.push(newLatLng)
                    })

                    if (response.length > 0) {
                        map.panTo(new google.maps.LatLng(response[0].latitude, response[0].longitude))
                    }
                    polyline.setPath(polylines)
                    polyline.setMap(map)
                },
                error: function (xhr, status, error) {
                    // Handle errors
                    $('#content').html('Error: ' + error)
                }
            })
            document.getElementById('history').textContent = 'Gecmisi Kapat'
        } else if (historyClicked == true) {
            // Marker'lar� haritadan kald�r
            markers.forEach(function (marker) {
                marker.setMap(null) // Marker'� haritadan kald�r
            })
            markers = [] // Marker listesini s�f�rla
            historyClicked = false
            document.getElementById('history').textContent = 'Gecmisi Goster'
            // Polyline'� haritadan kald�r
            polyline.setMap(null)
        }
    })
}
function loadNewHistoryLocation () {
    var position = marker.getPosition()
    var newLatLng = new google.maps.LatLng(position.lat(), position.lng())

    createNewMarker(newLatLng, marker.customTime)

    polylines.push(newLatLng)
    polyline.setPath(polylines)
}
function addMarker (newMarker, time) {
    var contentString = 'Konum: ' + newMarker.getPosition().toUrlValue(6) + '<br> Zaman: ' + moment(time).format('YYYY-MM-DD HH:mm:ss')
    newMarker.addListener('click', function () {
        infowindow.setContent(contentString)
        infowindow.open(map, newMarker)
    })
}
function createNewMarker (position, time) {
    newMarker = new google.maps.Marker({
        position: position,
        map: map,
        icon: {
            path: google.maps.SymbolPath.CIRCLE,  // K���k yuvarlak ikon
            scale: 5,  // �konun boyutu
            fillColor: '#ADD8E6',  // �konun rengi
            fillOpacity: 1.0,
            strokeColor: '#0000FF',
            strokeWeight: 1
        },
        title: 'Konum: ' + position.toUrlValue(6)
    })
    markers.push(newMarker)
    addMarker(newMarker, time)
}

function angleMarker (newMarker, oldMarker) {
    // Dereceleri radyana �evir
    const toRadians = deg => deg * (Math.PI / 180)
    // Ba�lang�� ve biti� koordinatlar�n� radyana �evir
    const lat1 = toRadians(oldMarker.lat())
    const lon1 = toRadians(oldMarker.lng())
    const lat2 = toRadians(newMarker.latitude)
    const lon2 = toRadians(newMarker.longitude)

    // Boylam fark�n� hesapla
    const dLon = lon2 - lon1

    // A��y� hesapla
    const y = Math.sin(dLon) * Math.cos(lat2)
    const x = Math.cos(lat1) * Math.sin(lat2) - Math.sin(lat1) * Math.cos(lat2) * Math.cos(dLon)
    let angle = Math.atan2(y, x)

    // Radyan� dereceye �evir
    angle = angle * (180 / Math.PI)

    // Negatif a��lar� pozitife �evir (0-360 aral���na getir)
    angle = (angle + 360) % 360
    return angle
}