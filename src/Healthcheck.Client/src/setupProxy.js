
module.exports = function(app) {
    const express = require('express');
const mockMiddleware = require('express-mock-middleware');
app.use(mockMiddleware({glob: 'mock/**/*.js'}));
app.listen(3000, function (err) {
  return console.log(err);
})
}
