window.chartColors = {
    red: 'rgb(255, 0, 0)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(234, 191, 20)',
    green: 'rgb(0, 233, 0)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};

function toggle() {
    let tables = document.getElementById('tables');
    let charts = document.getElementById('charts');
    let toggler = document.getElementById('toggler');

    if (toggler.checked == true) {
        tables.style.display = "none";
        charts.style.display = "block";

        let levelsBefore = [];
        let levelsAfter = [];
        let exercises = [];
        let carbs = [];

        $.each(JSON.parse(document.getElementById('bloodSugarsBefore').value), function (key, value) {
            levelsBefore.push({ x: key, y: value });
        });

        $.each(JSON.parse(document.getElementById('bloodSugarsAfter').value), function (key, value) {
            levelsAfter.push({ x: key, y: value });
        });

        $.each(JSON.parse(document.getElementById('exercises').value), function (key, value) {
            exercises.push({ x: key, y: value });
        });

        $.each(JSON.parse(document.getElementById('carbs').value), function (key, value) {
            carbs.push({ x: key, y: value });
        });

        createCharts(levelsBefore, levelsAfter, exercises, carbs);
    }
    else {
        charts.style.display = "none";
        tables.style.display = "block";
    }
}

function createCharts(levelsBefore, levelsAfter, exercises, carbs) {
    let bloodCtx = document.getElementById('bloodChart').getContext('2d');
    let carbCtx = document.getElementById('carbChart').getContext('2d');
    let exerciseCtx = document.getElementById('exerciseChart');

    if (bloodCtx != null) {
        let bloodChart = new Chart(bloodCtx, {
            type: 'line',
            data: {
                datasets: [{
                    label: 'Blood Sugar Readings Before',
                    borderColor: window.chartColors.red,
                    data: levelsBefore
                },
                {
                    label: 'Blood Sugar Readings After',
                    borderColor: window.chartColors.blue,
                    data: levelsAfter
                }]
            },
            options: {
                scales: {
                    xAxes: [{
                        type: 'time',
                        distribution: 'series',
                        position: 'bottom',
                        time: {
                            unit: 'day',
                            displayFormats: {
                                day: 'MMM D h:mm a'
                            }
                        }
                    }]
                }
            }
        })
    }

    if (exerciseCtx != null) {
        let exerciseChart = new Chart(exerciseCtx, {
            type: 'line',
            data: {
                datasets: [{
                    label: 'Exercise Times',
                    borderColor: window.chartColors.green,
                    data: exercises
                }]
            },
            options: {
                scales: {
                    xAxes: [{
                        type: 'time',
                        distribution: 'series',
                        position: 'bottom',
                        time: {
                            unit: 'day',
                            displayFormats: {
                                day: 'MMM D h:mm a'
                            }
                        }
                    }]
                }
            }
        })
    }

    if (carbCtx != null) {
        let carbChart = new Chart(carbCtx, {
            type: 'line',
            data: {
                datasets: [{
                    label: 'Carb Measurements',
                    borderColor: window.chartColors.yellow,
                    data: carbs
                }]
            },
            options: {
                scales: {
                    xAxes: [{
                        type: 'time',
                        distribution: 'series',
                        position: 'bottom',
                        time: {
                            unit: 'day',
                            displayFormats: {
                                day: 'MMM D h:mm a'
                            }
                        }
                    }]
                }
            }
        })
    }
}