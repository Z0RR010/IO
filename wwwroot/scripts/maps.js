export let maps = {};

// (g=>{var h,a,k,p="The Google Maps JavaScript API",c="google",l="importLibrary",q="__ib__",m=document,b=window;b=b[c]||(b[c]={});var d=b.maps||(b.maps={}),r=new Set,e=new URLSearchParams,u=()=>h||(h=new Promise(async(f,n)=>{await (a=m.createElement("script"));e.set("libraries",[...r]+"");for(k in g)e.set(k.replace(/[A-Z]/g,t=>"_"+t[0].toLowerCase()),g[k]);e.set("callback",c+".maps."+q);a.src=`https://maps.${c}apis.com/maps/api/js?`+e;d[q]=f;a.onerror=()=>h=n(Error(p+" could not load."));a.nonce=m.querySelector("script[nonce]")?.nonce||"";m.head.append(a)}));d[l]?console.warn(p+" only loads once. Ignoring:",g):d[l]=(f,...n)=>r.add(f)&&u().then(()=>d[l](f,...n))})({
//     key: "AIzaSyCaEHkCZC5zP2OjibM8Ri2I7D-1UoZLU8M",
//     v: "weekly",
// });

export function loadGoogleMapsAPI(options) {
    let h, a, k;
    const p = "The Google Maps JavaScript API";
    const c = "google";
    const l = "importLibrary";
    const q = "__ib__";
    const m = document;
    const b = window;
    const googleNamespace = b[c] || (b[c] = {});
    const mapsNamespace = googleNamespace.maps || (googleNamespace.maps = {});
    const r = new Set();
    const e = new URLSearchParams();

    const u = () =>
        h ||
        (h = new Promise(async (resolve, reject) => {
            a = m.createElement("script");
            e.set("libraries", [...r] + "");
            for (k in options) {
                e.set(k.replace(/[A-Z]/g, t => "_" + t[0].toLowerCase()), options[k]);
            }
            e.set("callback", `${c}.maps.${q}`);
            a.src = `https://maps.${c}apis.com/maps/api/js?${e}`;
            mapsNamespace[q] = resolve;
            a.onerror = () => (h = reject(new Error(`${p} could not load.`)));
            a.nonce = m.querySelector("script[nonce]")?.nonce || "";
            m.head.append(a);
        }));

    if (mapsNamespace[l]) {
        console.warn(`${p} only loads once. Ignoring.`);
    } else {
        mapsNamespace[l] = (feature, ...params) => {
            r.add(feature);
            return u().then(() => mapsNamespace[l](feature, ...params));
        };
    }
}


export async function initMap(mapId, lat, lng) {
    const position = { lat: lat, lng: lng };
    
    const { Map } = await google.maps.importLibrary("maps");
    
    maps[mapId] = new Map(document.getElementById(mapId), {
        zoom: 15,
        center: position,
        mapId: mapId,
    });
}

export async function addMarker(mapId, lat, lng, title) {
    const position = { lat: lat, lng: lng };

    const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");

    const marker = new AdvancedMarkerElement({
        map: maps[mapId],
        position: position,
        title: title,
    });
}

export async function geocode(address) {
    const { Geocoder } = await google.maps.importLibrary("geocoding");

    const geocoder = new Geocoder();

    return new Promise((resolve, reject) => {
        geocoder.geocode({ address: address }, (results, status) => {
            if (status === "OK") {
                const location = results[0].geometry.location;
                resolve({ Latitude: location.lat(), Longitude: location.lng() });
            } else {
                reject("Google Maps API Geocoding: " + status);
            }
        });
    });
}

const translations = {
    en: {
        distance: 'Total Distance',
        time: 'Estimated Time',
        unitDistance: 'km',
        unitTime: 'minutes',
    },
    pl: {
        distance: 'Całkowita odległość',
        time: 'Szacowany czas',
        unitDistance: 'km',
        unitTime: 'minuty',
    },
};
export async function drawRoute(mapId, routeCoordinates, language) {
    if (routeCoordinates.length < 2) {
        throw new Error("At least two coordinates are required to draw a route.");
    }

    const directionsService = new google.maps.DirectionsService();
    const directionsRenderer = new google.maps.DirectionsRenderer({
        map: maps[mapId]
    });

    const waypoints = routeCoordinates
        .slice(1, -1)
        .map(coord => ({
            location: new google.maps.LatLng(coord.lat, coord.lng),
            stopover: true,
        }));

    const request = {
        origin: new google.maps.LatLng(routeCoordinates[0].lat, routeCoordinates[0].lng),
        destination: new google.maps.LatLng(
            routeCoordinates[routeCoordinates.length - 1].lat,
            routeCoordinates[routeCoordinates.length - 1].lng
        ),
        waypoints: waypoints,
        travelMode: google.maps.TravelMode.DRIVING,
    };

    try {
        const result = await directionsService.route(request);
        directionsRenderer.setDirections(result);

        // Extracting route details (distance and duration)
        const route = result.routes[0];
        const legs = route.legs;

        let totalDistance = 0;
        let totalDuration = 0;

        legs.forEach(leg => {
            totalDistance += leg.distance.value; // distance in meters
            totalDuration += leg.duration.value; // duration in seconds
        });

        // Convert distance to kilometers and duration to minutes
        totalDistance = (totalDistance / 1000).toFixed(2); // km
        totalDuration = Math.ceil(totalDuration / 60); // minutes

        const infoDiv = document.createElement('div');
        infoDiv.style.backgroundColor = 'white';
        infoDiv.style.padding = '10px';
        infoDiv.style.margin = '10px';
        infoDiv.style.borderRadius = '5px';
        infoDiv.style.boxShadow = '0 2px 6px rgba(0,0,0,0.3)';

        const text = translations[language]

        infoDiv.innerHTML = `<strong>${text.distance}:</strong> ${totalDistance} ${text.unitDistance}<br><strong>${text.time}:</strong> ${totalDuration} ${text.unitTime}`;

        maps[mapId].controls[google.maps.ControlPosition.TOP_CENTER].push(infoDiv);

        return result;
    } catch (error) {
        throw new Error(`Directions request failed: ${error.message}`);
    }
}
