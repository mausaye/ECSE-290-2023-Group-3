const express = require('express')
const Scoreboard = require('./Scoreboard.js')
const port = 5050
const app = express()
app.use(express.json())

let scoreboard = new Scoreboard(5);

/*
        This is how this file will most frequently be called:
                1. At the end of the game, a POST request will be sent to the server, triggering the app.post(...) function.
                2. A list of 5 (or fewer) scores will be sent back to the player, corresponding to the top 5 scores.

        If we decided to have a way to look at the scoreboard without beating the game (maybe a button on the main menu or something),
        we'd want to retrieve the scoreboard without putting data into it. We can do this by sending a GET request from the game to the server,
        triggering the app.get(...) function. This function will just send back the data.
*/

app.get('/', (req, res) => {
        res.send(scoreboard.getJSON());

});

app.post('/', (req, res) => {
        const name = req.body.name;
        const score = req.body.score;
        scoreboard.addScore(name, score);
        res.send(scoreboard.getJSON());
});

app.listen(port, () => {
        console.log('We listening');
});