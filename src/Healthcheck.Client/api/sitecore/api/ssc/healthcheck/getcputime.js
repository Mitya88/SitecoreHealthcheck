module.exports = (req, res) => {
  const { name = 'World' } = req.query;
 let CpuTimeNumber = Math.floor(Math.random() * 100);
  res.status(200).json({"CpuTimeNumber":CpuTimeNumber,"CpuTimeText":CpuTimeNumber});
};