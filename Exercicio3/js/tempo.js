$(function () {

    //Colocar os nomes dos meses para não ser numeros
    const nomeMeses = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];

    //Popular o dropdown com as cidades
    $.getJSON('https://api.ipma.pt/open-data/distrits-islands.json', function (data) {
        for (index in data.data) {
            $('#controlSelect1').append('<option value="' + data.data[index].globalIdLocal + '">' + data.data[index].local + '</option>');
        }

    });

    var temperaturas;
    //Quando selecionado a cidade apresentar o tempo maximo e o restantes dias
    $("#controlSelect1").change(function () {
        val = $("#controlSelect1").val();

        //Guardar em val o globalIdLocal para assim buscarmos os JSON da cidade em questão
        $.getJSON('https://api.ipma.pt/open-data/forecast/meteorology/cities/daily/' + val + '.json', function (data) {
            for (i in data.data) {
                $("#dias").remove();
            }
            $("#temp").remove();
            $('#today').append('<span id="temp" class="tempPrincipal" value="' + data.data[0].tMax + '">' + data.data[0].tMax + '°</span>');


            $.getJSON('https://api.ipma.pt/open-data/weather-type-classe.json', function (weatherType) {
                console.log(weatherType)
                $("#typeWea").remove();
                $("#btnD").remove();
                $("#downloadJSON").remove();
                $('#typeWeather').append('<div id="typeWea"> <img class="imgCenter" src="img/' + data.data[0].idWeatherType + '.png " alt=""></div>');
                $('#btnDays').append('<button id="btnD" type="button" class="btn btnBlack ">5 Dias</button>');
                $('#btnJSON').append(' <button id="downloadJSON" type="button" class="btn btn-outline-light">Download JSON</button>');
                temperaturas = {
                    temperaturas: []
                };
                //Percorrer os 5 dias para nos dar a temperatura para os 5dias e estado do tempo chuva,sol,aguaceiros, etc....
                for (i in data.data) {

                    var dateObj = new Date(data.data[i].forecastDate);
                    var mes = dateObj.getUTCMonth();
                    var dia = dateObj.getUTCDate();
                    $('#diasDaSemana').append('<div id="dias" class="col-md-2 textCenter" ><span><b>' + dia + " " + nomeMeses[mes] + '</b></span><br><br><span class="maxTemp">' + data.data[i].tMax + '°</span><br><span  class="minTemp">' + data.data[i].tMin + "°</span><br>" + '<img class="imgCenter" src="img/' + data.data[i].idWeatherType + '.png " alt=""></img></div>');


                    //fazer um objeto de temperaturas onde vamos colocar a temperatura MAX e MIN e fazer o calculo da média
                    temperaturas.temperaturas.push({
                        min: parseFloat(data.data[i].tMin),
                        max: parseFloat(data.data[i].tMax),
                        media: Math.round((parseFloat(data.data[i].tMin) + parseFloat(data.data[i].tMax)) / 2, 2),
                    })
                }


                //Fazer download de um ficheiro JSON da temperatura MAX, temperatura MIN e temperatura média
                $("#downloadJSON").click(function () {
                    $("<a />", {
                            "download": "temperatura.json",
                            "href": "data:application/json," + encodeURIComponent(JSON.stringify(temperaturas))
                        }).appendTo("body")
                        .click(function () {
                            $(this).remove()
                        })[0].click()
                });
            });
        });
    });
});