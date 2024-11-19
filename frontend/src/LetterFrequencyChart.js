import React, { useState } from 'react';
import { Bar } from 'react-chartjs-2';
import axios from 'axios';
import './LetterFrequencyChart.css';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';

// Register the components
ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

const LetterFrequencyChart = () => {
  const [chartData, setChartData] = useState({
    labels: [],
    datasets: [
      {
        label: 'Letter Frequency',
        data: [],
        backgroundColor: 'rgba(0, 112, 198, 0.6)', // Change to #0070c6 with opacity
        borderColor: '#0070c6', // Change to #0070c6
        borderWidth: 1,
      },
    ],
  });
  const [repoUrl, setRepoUrl] = useState('https://github.com/lodash/lodash'); // Set default value here
  const [loading, setLoading] = useState(false); // Add loading state

  const fetchData = async (url) => {
    setLoading(true); // Set loading to true before fetching data
    try {
      const response = await axios.post(process.env.REACT_APP_STATS_ENDPOINT, { repositoryUrl: url });
      const data = response.data;
      const labels = Object.keys(data);
      const values = Object.values(data);

      setChartData({
        labels: labels,
        datasets: [
          {
            label: 'Letter Frequency',
            data: values,
            backgroundColor: 'rgba(0, 112, 198, 0.6)', // Change to #0070c6 with opacity
            borderColor: '#0070c6', // Change to #0070c6
            borderWidth: 1,
          },
        ],
      });
    } catch (error) {
      console.error('Error fetching data:', error);
    } finally {
      setLoading(false); // Set loading to false after fetching data
    }
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    fetchData(repoUrl);
  };

  return (
    <div className="container">
      <h2>Repo Insight</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={repoUrl}
          onChange={(e) => setRepoUrl(e.target.value)}
          placeholder="Enter repository URL"
          required
        />
        <button type="submit">Analyze</button>
      </form>
      {loading ? (
        <div className="loader"></div> // Show loader while loading
      ) : (
        <div className="chart-container">
          <Bar
            data={chartData}
            options={{
              scales: {
                y: {
                  beginAtZero: true,
                },
              },
            }}
          />
        </div>
      )}
    </div>
  );
};

export default LetterFrequencyChart;