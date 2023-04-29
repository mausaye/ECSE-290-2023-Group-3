var PriorityQueue = require('priorityqueuejs');
const capacity = 5;


function Scoreboard(topK) {
    /*
        - library only supports minqueue, so the negative version of elements will be pushed in.
        - So if we want (5, 4, 3, 2, 1) to be the scoreboard, the queue with have (-5, -4, -3, -2, -1)
          and then the negative version of each element will be sent over.
    */
    this.capacity = topK;
    this.q = new PriorityQueue(function(a, b) {
        return b.score - a.score;
    });

    //initialize with bad scores that can easily be beaten.
    this.addScore('Kim', 1000);
    this.addScore('Tammy', 1050);
    this.addScore('Quyen', 3401);
    this.addScore('Robbie', 235);
    this.addScore('Sally', 40582);
}

Scoreboard.prototype.addScore = function(name, score) {
    console.log({name, score});

    // add new record into queue
    var record = {
        name,
        score,
    }
    this.q.enq(record);

    // if there are more than 5 record in queue, 
    //    pop the first 5 record in queue to data
    //    delete the rest in queue
    //    add the current top 5 into queue
    if (this.q.size() > this.capacity) {
        
        const data = [];

        for (let i = 0; i < this.capacity; i++){
            data.push(this.q.deq());
        }

        this.q.deq();

        for (let i = 0; i < data.length; i++){
            this.q.enq(data[i]);
        }
    }
}

Scoreboard.prototype.getJSON = function() {
    //pop all from q, save to list.
    var data = [];
    while (this.q.size() > 0) {
        data.push(this.q.deq());
    }

    for (var i = 0; i < data.length; i++) {
        this.q.enq(data[i]);
    }
    return JSON.stringify(data);
}

module.exports = Scoreboard;