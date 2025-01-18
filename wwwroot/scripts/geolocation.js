export async function getUserLocation() {
    const pos = await new Promise((resolve, reject) => {
        navigator.geolocation.getCurrentPosition(resolve, reject);
    });
    
    return { Latitude: pos.coords.latitude, Longitude: pos.coords.longitude };
}