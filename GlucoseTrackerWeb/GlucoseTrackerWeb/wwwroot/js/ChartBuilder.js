function toggle() {
    let tables = document.getElementById('tables');
    let charts = document.getElementById('charts');
    let toggler = document.getElementById('toggler');
    if (toggler.checked == true) {
        tables.style.display = "none";
        charts.style.display = "block";
        createCharts();
    }
    else {
        charts.style.display = "none";
        tables.style.display = "block";
    }
}

function createCharts() {
    let bloodChart = document.getElementById('bloodChart').getContext('2d');
    let carbChart = document.getElementById('carbChart').getContext('2d');
    let exerciseChart = document.getElementById('exerciseChart').getContext('2d');
    new Chart(bloodChart, {
        type: 'line',
        data: {
            datasets: [{
                label: 'Blood Sugar Readings',
                data: [{
                    x: -10,
                    y: 0
                }, {
                    x: 0,
                    y: 10
                }, {
                    x: 10,
                    y: 5
                }]
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    type: 'linear',
                    position: 'bottom'
                }]
            }
        }
    })

    new Chart(exerciseChart, {
        type: 'line',
        data: {
            datasets: [{
                label: 'Blood Sugar Readings',
                data: [{
                    x: -10,
                    y: 0
                }, {
                    x: 0,
                    y: 10
                }, {
                    x: 10,
                    y: 5
                }]
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    type: 'linear',
                    position: 'bottom'
                }]
            }
        }
    })

    new Chart(carbChart, {
        type: 'line',
        data: {
            datasets: [{
                label: 'Blood Sugar Readings',
                data: [{
                    x: -10,
                    y: 0
                }, {
                    x: 0,
                    y: 10
                }, {
                    x: 10,
                    y: 5
                }]
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    type: 'linear',
                    position: 'bottom'
                }]
            }
        }
    })
}