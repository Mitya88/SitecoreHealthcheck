import React from 'react';
import { Bar } from 'react-chartjs-2';

class ErrorChart extends React.Component {

  state = {
    errorStats: {},
    isLoading: true
  }

  componentDidMount() {
    this.fetchErrorStatistics();
  }

  fetchErrorStatistics() {
    fetch('/sitecore/api/ssc/healthcheck/geterrorcounts?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ errorStats: data, isLoading: false });
      });
  }

  componentDidUpdate() {
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

        var text = 'Error counts',
          textX = Math.round((width - ctx.measureText(text).width) / 2),
          textY = height / 2;

        ctx.fillText(text, textX, textY);
        ctx.save();
      }
    }];

    let labels = this.state.errorStats.Dates;
    let errorData = this.state.errorStats.Counts;
    const data = {
      labels: labels,
      datasets: [{
        label: 'Error counts', // Name the series
        data: errorData, // Specify the data values array
        fill: false,
        borderColor: 'red', // Add custom color border (Line)
        backgroundColor: 'red', // Add custom color background (Points and Fill)
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
            suggestedMin: 0,
            stepSize: 2
          }
        }]
      }
    };

    return (

      <div className="col-sm-3">
        <div className="card mb-3">
          <h5 className="center">Error Counts per days</h5>
          <div className="card-block main-menu-item height-270">
            <Bar data={data} options={options} plugins={plugins} />
          </div>
        </div>
      </div>
    );
  }
}

export default ErrorChart;
