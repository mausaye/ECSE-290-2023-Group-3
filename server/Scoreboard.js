const {MinQueue} = require("heapify")

function Scoreboard(topK) {
    this.topK = topK;
    /*
        - library only supports minqueue, so the negative version of elements will be pushed in.
        - So if we want (5, 4, 3, 2, 1) to be the scoreboard, the queue with have (-5, -4, -3, -2, -1)
          and then the negative version of each element will be sent over.
    */
    this.q = new MinQueue(5);

    this.testQ()
}

Scoreboard.prototype.testQ = function() {
    this.addScore(4)
    this.addScore(5)
    this.addScore(3)
    this.addScore(2)
    this.addScore(1)
}

Scoreboard.prototype.addScore = function(score) {
    this.q.push(score, -1 * score);
}

Scoreboard.prototype.getJSON = function() {
    //pop all from q, save to list.
    var data = [];
    while (this.q.size > 0) {
        data.push(this.q.peek());
        this.q.pop();
    }

    for (var i = 0; i < data.length; i++) {
        this.q.push(data[i], -1 * data[i]);
    }
    return JSON.stringify(data);
}

module.exports = Scoreboard;