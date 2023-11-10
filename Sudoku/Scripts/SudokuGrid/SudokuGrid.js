let sudokuArr;
var dict;

function drawGrid(className, list, rows, columns, editState) {
    sudokuArr = list
    var target = '.' + className;
    const query = document.querySelector(target)
    for (let i = 1; i <= rows; i++) {
        for (let j = 1; j <= columns; j++) {

            var btn = document.createElement('button')
            btn.id = i + ' ' + j;
            btn.type = 'button'

            btn.readOnly = true
            btn.classList.add('box')

            if (rows == 9 && columns == 9) {

                addEventToBtn(btn, list, i, j, 'black', editState)

                if ((i < 4 || i > 6) && (j < 4 || j > 6)) {
                    btn.classList.add('border-upper')
                }
                else if ((i >= 4 && i <= 6 && j >= 4 && j <= 6)) {
                    btn.classList.add('border-upper')
                }
            }
            else {
                btn.classList.add('border-upper')
                initalizeDict(rows);
                var key = (i - 1) + " " + (j - 1);

                if (dict[key] == 1) {
                    btn.style.color = 'blue'
                    addEventToBtn(btn, list, i, j, 'blue', editState)
                }
                else if (dict[key] == 2) {
                    btn.style.color = 'green'
                    addEventToBtn(btn, list, i, j, 'green', editState)

                }
                else if (dict[key] == 3) {
                    btn.style.color = 'burlywood'
                    addEventToBtn(btn, list, i, j, 'burlywood', editState)

                }
                else if (dict[key] == 4) {
                    btn.style.color = 'black'
                    addEventToBtn(btn, list, i, j, 'black', editState)
                }
                else if (dict[key] == 5) {
                    btn.style.color = 'cornflowerblue'
                    addEventToBtn(btn, list, i, j, 'cornflowerblue', editState)

                }
                else if (dict[key] == 6) {
                    btn.style.color = 'firebrick'
                    addEventToBtn(btn, list, i, j, 'firebrick', editState)
                }
                else if (dict[key] == 7) {
                    btn.style.color = 'blueviolet'
                    addEventToBtn(btn, list, i, j, 'blueviolet', editState)
                }
            }
            query.append(btn)
        }
        var br = document.createElement('br')
        query.append(br)
    }
}

function btnEvent(e, i, j, color) {
    console.log(color)
    e.target.style.color = color
    //adjust riddle reach first condition or not and modify it value
    if (e.target.textContent == '_') {
        e.target.textContent = 1;
        acceptRiddle--;
    }
    else {
        e.target.textContent = parseInt(e.target.textContent) + 1
    }

    if (e.target.textContent == rows + 1) {
        sudokuArr[i][j] = 0;
        e.target.textContent = '_'
        acceptRiddle++;
    }
    else {
        sudokuArr[i][j] = e.target.textContent;
    }

    //disable based on condition
    if (acceptRiddle <= 0) {
        document.getElementById('test').disabled = false;
    }
    else {
        document.getElementById('test').disabled = true;
    }
    document.getElementById('submit').disabled = true;

    e.preventDefault()
}
function initalizeDict(rows) {
    if (rows == 5) {
        dict = {
            //box 1
            "0 0": 1,
            "0 1": 1,
            "0 2": 1,
            "1 0": 1,
            "1 1": 1,
            //box 2
            "0 3": 2,
            "0 4": 2,
            "1 3": 2,
            "1 4": 2,
            "2 4": 2,
            //box 3
            "2 0": 3,
            "3 0": 3,
            "3 1": 3,
            "4 0": 3,
            "4 1": 3,
            //box 4
            "1 2": 4,
            "2 1": 4,
            "2 2": 4,
            "2 3": 4,
            "3 2": 4,
            //box 5
            "3 3": 5,
            "3 4": 5,
            "4 2": 5,
            "4 3": 5,
            "4 4": 5,
        }
    }
    else if (rows == 7) {
        dict = {
            //box 1
            "0 0": 1,
            "0 1": 1,
            "0 2": 1,
            "0 3": 1,
            "1 0": 1,
            "1 1": 1,
            "1 2": 1,
            //box 2
            "0 4": 2,
            "0 5": 2,
            "0 6": 2,
            "1 3": 2,
            "1 4": 2,
            "1 5": 2,
            "1 6": 2,
            //box 3
            "2 0": 3,
            "2 1": 3,
            "3 0": 3,
            "3 1": 3,
            "3 2": 3,
            "4 1": 3,
            "4 2": 3,
            //box 4
            "2 2": 4,
            "2 3": 4,
            "3 3": 4,
            "4 3": 4,
            "4 4": 4,
            "4 5": 4,
            "5 3": 4,
            //box 5
            "2 4": 5,
            "2 5": 5,
            "2 6": 5,
            "3 4": 5,
            "3 5": 5,
            "3 6": 5,
            "4 6": 5,
            //box 6
            "4 0": 6,
            "5 0": 6,
            "5 1": 6,
            "5 2": 6,
            "6 0": 6,
            "6 1": 6,
            "6 2": 6,
            //box 7
            "5 4": 7,
            "5 5": 7,
            "5 6": 7,
            "6 3": 7,
            "6 4": 7,
            "6 5": 7,
            "6 6": 7,
        }
    }
}

function addEventToBtn(btn, list, i, j, color, editState) {
    if (list[i - 1][j - 1] == 0) {
        btn.textContent = '_'
        btn.addEventListener('click', function (e) {
            btnEvent(e, i - 1, j - 1, color);
        })
    }
    else {
        acceptRiddle--;
        btn.textContent = list[i - 1][j - 1];
        btn.style.fontWeight = '700';


        if (editState == true) {
            btn.addEventListener('click', function (e) {
                btnEvent(e, i - 1, j - 1, color);
            })
        }

    }
}