import React from 'react';
import { Doughnut, Line, Bar } from 'react-chartjs-2';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class CpuChart extends React.Component {

  state = {
    cpuStatistics: {},
    isLoading: true
  }

  timer = null;
  componentDidMount() {
    this.timer = setInterval(() => {
      this.fetchCpu();
    }, 2000);
  }

  
  componentWillUnmount(){
    clearInterval(this.timer);
  }
  

  fetchCpu() {
    fetch('/sitecore/api/ssc/healthcheck/getcputime?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ cpuStatistics: data, isLoading: false });
      });
  }

  render() {
    let cpuTime = Math.round(this.state.cpuStatistics.CpuTimeNumber);
    let freeCapactiy = 100 - cpuTime;

    let plugins = [{
      beforeDraw: function (chart) {
        var width = chart.chart.width,
          height = chart.chart.height,
          ctx = chart.chart.ctx;

        ctx.restore();
        var fontSize = (height / 200).toFixed(2);
        ctx.font = fontSize + "em sans-serif";
        ctx.textBaseline = "middle";

        var text = 'CPU',
          textX = Math.round((width - ctx.measureText(text).width) / 2),
          textY = height / 2;

        var text2 = 'Usage',
          textX2 = Math.round((width - ctx.measureText(text2).width) / 2);

        ctx.fillText(text, textX, textY);
        ctx.fillText(text2, textX2, textY + 20);
        ctx.save();
      }
    }];

    const data = {
      labels: [
        'CPU Usage',
        'Free'
      ],
      datasets: [{
        data: [cpuTime, freeCapactiy],
        backgroundColor: [
          'rgb(69, 74, 117)',
          '#bfe1fa'
        ],
        hoverBackgroundColor: [
          'rgb(69, 74, 117)',
          '#bfe1fa',
        ]
      }]
    };

    const options = {
      responsive: true,
      legend: {
        display: false
      }
    };

    return (
      <div className="col-sm-3">
        <div className="card mb-3">
          <h5 className="center">CPU Usage {Math.round(this.state.cpuStatistics.CpuTimeText)}%</h5>
          <div className="card-block main-menu-item height-270">
            {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
              <Doughnut data={data} options={options} plugins={plugins} />
            }
          </div>
        </div>
      </div>
    );
  }
}

export default CpuChart;
