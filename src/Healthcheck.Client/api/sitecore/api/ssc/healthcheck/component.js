module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json({"Id":"1","Name":"Database","ErrorCount":1,"Status":"Warning","LastCheckTime":"0001-01-01T00:00:00","ErrorList":{"Entries":[]}});
};