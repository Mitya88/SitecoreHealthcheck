import React from 'react';
import { Doughnut } from 'react-chartjs-2';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class DriveInfoChart extends React.Component {

  state = {
    driveStatistics: {},
    isLoading: true
  }

  componentDidMount() {
    fetch('/api/sitecore/api/ssc/healthcheck/getdriveinfo?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ driveStatistics: data, isLoading: false });
      });
  }

  render() {

    var freeSize = this.state.driveStatistics.FreeCapacity;
    var usedSize = this.state.driveStatistics.UsedCapacity;

    let plugins = [{
      beforeDraw: function (chart) {
        var width = chart.chart.width,
          height = chart.chart.height,
          ctx = chart.chart.ctx;

        ctx.restore();
        var fontSize = (height / 200).toFixed(2);
        ctx.font = fontSize + "em sans-serif";
        ctx.textBaseline = "middle";

        var text = 'Disk Space in MB',
          textX = Math.round((width - ctx.measureText(text).width) / 2),
          textY = height / 2;

        ctx.fillText(text, textX, textY);
        ctx.save();
      }
    }];

    const data = {
      labels: [
        'Disk Space Used',
        'Free Disk Space'
      ],
      datasets: [{
        data: [usedSize, freeSize],
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

    let title = this.state.isLoading === true ? <h5 className="center">Disk Details</h5> : <h5 className="center">Sitecore is hosted on <strong>{this.state.driveStatistics.DriveLetter}</strong> (Free: {this.state.driveStatistics.FreeCapacityText}, Used: {this.state.driveStatistics.UsedCapacityText})</h5>

    return (
      <div className="col-sm-3">
        <div className="card mb-3">

          {title}
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

export default DriveInfoChart;
