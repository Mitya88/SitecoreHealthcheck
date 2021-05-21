import React from 'react';
import { Doughnut } from 'react-chartjs-2';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class ComponentChart extends React.Component {

  state = {
    componentStatistics: {},
    isLoading: true
  }

  componentDidMount() {
    this.fetchComponentStatistics();
  }

  fetchComponentStatistics() {
    fetch('/sitecore/api/ssc/healthcheck/getcomponentstatistics?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ componentStatistics: data, isLoading: false });
      });
  }

  render() {

    let plugins = [{
      beforeDraw: function (chart) {
        var width = chart.chart.width,
          height = chart.chart.height,
          ctx = chart.chart.ctx;

        ctx.restore();
        var fontSize = (height / 250).toFixed(2);
        ctx.font = fontSize + "em sans-serif";
        ctx.textBaseline = "middle";

        var text = 'Component',
          textX = Math.round((width - ctx.measureText(text).width) / 2),
          textY = height / 2;

        var text2 = 'Stat',
          textX2 = Math.round((width - ctx.measureText(text2).width) / 2);

        // ctx.fillText(text, textX, textY);
        // ctx.fillText(text2, textX2, textY + 20);
        ctx.save();
      }
    }];

    const data = {
      labels: [
        'Healthy',
        'Warning',
        'Error',
        'Unknown'
      ],
      datasets: [{
        data: [this.state.componentStatistics.HealthyCount, this.state.componentStatistics.WarningCount, this.state.componentStatistics.ErrorCount, this.state.componentStatistics.UnknownCount],
        backgroundColor: [
          'rgb(69, 74, 117)',
          '#c3ecf6',
          '#c6e9e8',
          '#c1d2e8'
        ],
        hoverBackgroundColor: [
          'rgb(69, 74, 117)',
          '#c3ecf6',
          '#c6e9e8',
          '#c1d2e8'
        ]
      }]
    };

    const options = {
      responsive: true,
      legend: {
        display: true
      }
    };

    return (

      <div className="col-sm-3">
        <div className="card mb-3">
          <h5 className="center">Component Statistics</h5>
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

export default ComponentChart;
