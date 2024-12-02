let maps = {};

(g=>{var h,a,k,p="The Google Maps JavaScript API",c="google",l="importLibrary",q="__ib__",m=document,b=window;b=b[c]||(b[c]={});var d=b.maps||(b.maps={}),r=new Set,e=new URLSearchParams,u=()=>h||(h=new Promise(async(f,n)=>{await (a=m.createElement("script"));e.set("libraries",[...r]+"");for(k in g)e.set(k.replace(/[A-Z]/g,t=>"_"+t[0].toLowerCase()),g[k]);e.set("callback",c+".maps."+q);a.src=`https://maps.${c}apis.com/maps/api/js?`+e;d[q]=f;a.onerror=()=>h=n(Error(p+" could not load."));a.nonce=m.querySelector("script[nonce]")?.nonce||"";m.head.append(a)}));d[l]?console.warn(p+" only loads once. Ignoring:",g):d[l]=(f,...n)=>r.add(f)&&u().then(()=>d[l](f,...n))})({
    key: "AIzaSyCaEHkCZC5zP2OjibM8Ri2I7D-1UoZLU8M",
    v: "weekly",
});


async function initMap(mapId, lat, lng) {
    const position = { lat: lat, lng: lng };
    
    const { Map } = await google.maps.importLibrary("maps");
    
    maps[mapId] = new Map(document.getElementById(mapId), {
        zoom: 15,
        center: position,
        mapId: mapId,
    });
}

async function addMarker(mapId, lat, lng, title) {
    const position = { lat: lat, lng: lng };

    const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");

    const marker = new AdvancedMarkerElement({
        map: maps[mapId],
        position: position,
        title: title,
    });
}

async function geocode(address) {
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


