import React from 'react';
import { Line } from 'react-chartjs-2';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class MemoryChart extends React.Component {

  state = {
    memoryStatistics: {},
    isLoading: true
  }

  componentDidMount() {
    setInterval(() => {
      this.fetchMemory();
    }, 2000);
  }

  fetchMemory() {
    fetch('/sitecore/api/ssc/healthcheck/getmemoryusage?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.memoryData.push(data.MemoryUsageNumber);
        this.setState({ memoryStatistics: data, isLoading: false });
      });
  }
  memoryData = [];
  xAxis = [];

  componentDidUpdate() {
    if (this.memoryData.length > 10) {
      this.memoryData.shift();
    }

    if (this.xAxis.length > 10) {
      this.xAxis.shift();
    }

    // this.memoryData.push(this.state.memoryStatistics.MemoryUsageNumber);
    this.xAxis.push(1);
  }
  render() {
    let plugins = [{
      beforeDraw: function (chart) {
        var width = chart.chart.width,
          height = chart.chart.height,
          ctx = chart.chart.ctx;

        ctx.restore();
        var fontSize = (height / 200).toFixed(2);
        ctx.font = fontSize + "em sans-serif";
        ctx.textBaseline = "middle";

        var text = 'Memory usage',
          textX = Math.round((width - ctx.measureText(text).width) / 2),
          textY = height / 2;

        ctx.fillText(text, textX, textY);
        ctx.save();
      }
    }];


    const data = {
      labels: this.xAxis,
      datasets: [{
        label: 'Series 1', // Name the series
        data: this.memoryData, // Specify the data values array
        fill: false,
        borderColor: 'rgb(69, 74, 117)', // Add custom color border (Line)
        backgroundColor: 'rgb(69, 74, 117)', // Add custom color background (Points and Fill)
        borderWidth: 1 // Specify bar border width
      }]
    };

    const options = {
      responsive: true,
      legend: {
        display: false
      },
      scales: {
        yAxes: [{
          ticks: {
            stepSize: 60,
            suggestedMin: 0,
            suggestedMax: 900
          }
        }]
      }
    };

    return (
      <div className="col-sm-3">
        <div className="card mb-3">
          <h5 className="center">Memory Usage {this.state.memoryStatistics.MemoryUsageText}</h5>
          <div className="card-block main-menu-item height-270">
            {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
              <Line data={data} options={options} plugins={plugins} />
            }
          </div>
        </div>
      </div>
    );
  }
}

export default MemoryChart;
