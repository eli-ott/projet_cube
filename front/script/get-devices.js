/**
 * Get all the devices
 */
const getDevices = async () => {
    const res = await fetch('http://localhost:5023/device', {
        method: 'GET'
    });
    const ret = await res.json();
}