module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json({"DriveLetter":"c:\\","FreeCapacity":33633,"UsedCapacity":204218,"FreeCapacityText":"32.8 GB","UsedCapacityText":"199.4 GB"});
};