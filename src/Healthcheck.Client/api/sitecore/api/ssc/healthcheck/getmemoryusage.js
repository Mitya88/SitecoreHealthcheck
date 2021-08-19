module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  
  let memoryusage = Math.floor(Math.random() * 8000);
 let memoryusageText = (memoryusage / 1000) + " GB";
  res.status(200).json({"MemoryUsageNumber":memoryusage,"MemoryUsageText":memoryusageText});
};