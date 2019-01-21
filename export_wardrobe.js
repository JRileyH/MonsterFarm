app.displayDialogs = DialogModes.NO;
var rootDirectory = app.activeDocument.fullName.path+"/MonsterFarm/Content/Entities/Races";
var data = {};
var genderMap = {
    "m":"Male",
    "f":"Female",
    "male":"Male",
    "female":"Female"
}

function hideAllLayers(doc){
    for(var i = 0; i < doc.layers.length; i++){
        var layer = doc.layers[i];
        layer.visible = false;
        if(layer.typename==="LayerSet"){
            hideAllLayers(layer);
        }
    }
}

function extractData (doc, data){
    for(var i = 0; i < doc.layers.length; i++){
        var race = doc.layers[i];
        if(race.typename==="LayerSet"){
            var raceInfo = race.name.split('-');
            var raceName = raceInfo[0];
            var raceGender = genderMap[raceInfo[1].toLowerCase()];
            if(!data.hasOwnProperty(raceName)){
                data[raceName] = {
                    "Male":{
                        "Hat":{},
                        "Hair":{},
                        "Shirt":{},
                        "Pants":{},
                        "Shoes":{},
                        "Skin":{}
                    },
                    "Female":{
                        "Hat":{},
                        "Hair":{},
                        "Shirt":{},
                        "Pants":{},
                        "Shoes":{},
                        "Skin":{}
                    }
                };
            }
            var raceData = data[raceName][raceGender];
            for(var j = 0; j < race.layers.length; j++){
                var article = race.layers[j];
                if(article.typename==="LayerSet"){
                    if(raceData.hasOwnProperty(article.name)){
                        var articleData = raceData[article.name];
                        for(var k = 0; k < article.layers.length; k++){
                            var sheet = article.layers[k];
                            if(sheet.typename==="ArtLayer" && sheet.name.indexOf('gs-') == 0){
                                var sheetName = sheet.name.split('gs-')[1];
                                articleData[sheetName] = [
                                    race,
                                    article,
                                    sheet
                                ];
                            }
                        }
                    }
                }
            }
        }
    }
}

function exportData(doc, root, data){
    var rootFolder = Folder(root);
    if(!rootFolder.exists) rootFolder.create();
    for(var race in data){
        var raceData = data[race];
        var raceDirectory = root+"/"+race;
        var raceFolder = Folder(raceDirectory);
        if(!raceFolder.exists) raceFolder.create();
        for(var gender in raceData){
            var genderData = raceData[gender];
            var genderDirectory = raceDirectory+"/"+gender;
            var genderFolder = Folder(genderDirectory);
            if(!genderFolder.exists) genderFolder.create();
            for(var article in genderData){
                var articleData = genderData[article];
                var articleDirectory = genderDirectory+"/"+article;
                var articleFolder = Folder(articleDirectory);
                if(!articleFolder.exists) articleFolder.create();
                for(var sheet in articleData){
                    var imageLayers = articleData[sheet];
                    hideAllLayers(app.activeDocument);
                    for(var layer in imageLayers){
                        imageLayers[layer].visible = true;
                    }
                    var sheetFile = new File(articleDirectory+"/"+sheet+".png");
                    var opts = new PNGSaveOptions();
                    opts.compression = 0;
                    opts.interlaced = false;
                    doc.saveAs(sheetFile, opts, false, Extension.LOWERCASE);
                }
            }
        }
    }
}

extractData(app.activeDocument, data);
exportData(app.activeDocument, rootDirectory, data);