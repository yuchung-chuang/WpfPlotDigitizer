# Corpus scoreboard

Baseline pipeline version: `1`
Tolerance: `0.50%` median error, per series
Axis readings: `.github/skills/review-extraction/corpus-axis-readings.json`

Skill source versions (SHA-256 prefixes):
- `analyse-chart/inspect_chart.py`: `ada94f1d049d`
- `analyse-axis/locate_plot_area.py`: `7bef57519aa5`
- `analyse-axis/read_axis_scale.py`: `3c46418a71b0`
- `filter-noise/clear_border.py`: `357fbda74c3b`
- `segment-data/find_legend.py`: `05e787e92921`
- `segment-data/profile_series_styles.py`: `e91f0586df1e`
- `segment-data/segment_series.py`: `fa55983dc984`
- `segment-data/extract_series.py`: `25f1081a39c9`
- `export-data/export_data.py`: `a9dd8eecb89f`
- `review-extraction/score_extraction.py`: `60d267795719`

Median and p95 are the worst matched-series values for each figure; coverage is the mean.

| Figure | Series found | Median | P95 | Coverage | Result |
| --- | ---: | ---: | ---: | ---: | --- |
| Screenshot 2024-09-15 131234.png | 4/4 | 0.07% | 0.49% | 98.21% | PASS |
| Screenshot 2024-09-15 131309.png | 5/6 | 14.22% | 45.85% | 74.29% | FAIL: 4 of 6 series passed |
| Screenshot 2024-09-15 131341.png | 2/2 | 3.18% | 6.31% | 50.00% | FAIL: 1 of 2 series passed |
| Screenshot 2024-09-15 131401.png | 3/3 | 0.09% | 0.15% | 100.00% | PASS |
| Screenshot 2024-09-15 131423.png | 1/6 | 6.50% | 20.42% | 4.08% | FAIL: 0 of 6 series passed |
| Screenshot 2024-09-15 131439.png | 2/2 | 0.08% | 0.57% | 94.90% | PASS |
| Screenshot 2024-09-15 131522.png | 8/8 | 5.57% | 10.79% | 22.06% | FAIL: 2 of 8 series passed |
| Screenshot 2024-09-15 131556.png | 15/14 | 2.47% | 22.52% | 69.70% | FAIL: 10 of 14 series passed |
| Screenshot 2024-09-15 131624.png | 12/15 | 3.46% | 8.30% | 80.93% | FAIL: 10 of 15 series passed |
| Screenshot 2024-09-15 131643.png | 2/2 | 0.12% | 0.15% | 100.00% | PASS |
| Screenshot 2024-09-15 131712.png | 3/3 | 102.93% | 285.31% | 69.85% | FAIL: 2 of 3 series passed |
| Screenshot 2024-09-15 131740.png | 2/2 | 0.93% | 3.05% | 58.33% | FAIL: 1 of 2 series passed |
| Screenshot 2024-09-15 131805.png | 2/2 | 0.33% | 2.32% | 58.33% | PASS |

**Total:** 43 of 69 series passed.
