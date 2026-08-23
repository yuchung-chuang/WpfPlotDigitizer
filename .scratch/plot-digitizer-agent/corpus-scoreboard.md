# Corpus scoreboard

Baseline pipeline version: `1`
Tolerance: `0.50%` median error, per series
Axis readings: `.github/skills/review-extraction/corpus-axis-readings.json`

Skill source versions (SHA-256 prefixes):
- `analyse-chart/inspect_chart.py`: `ada94f1d049d`
- `analyse-axis/locate_plot_area.py`: `7bef57519aa5`
- `analyse-axis/read_axis_scale.py`: `ba79dddd5b25`
- `filter-noise/clear_border.py`: `357fbda74c3b`
- `segment-data/extract_point_series.py`: `e737cda2513b`
- `export-data/export_data.py`: `23342602ae70`
- `review-extraction/score_extraction.py`: `eb360bef4bf0`

Median and p95 are the worst matched-series values for each figure; coverage is the mean.

| Figure | Series found | Median | P95 | Coverage | Result |
| --- | ---: | ---: | ---: | ---: | --- |
| Screenshot 2024-09-15 131234.png | 1/4 | 7.38% | 15.51% | 0.00% | FAIL: 0 of 4 series passed |
| Screenshot 2024-09-15 131309.png | 1/6 | 0.53% | 7.64% | 42.86% | FAIL: 0 of 6 series passed |
| Screenshot 2024-09-15 131341.png | 1/2 | 7.93% | 13.37% | 0.00% | FAIL: 0 of 2 series passed |
| Screenshot 2024-09-15 131401.png | 1/3 | 3.04% | 8.05% | 12.24% | FAIL: 0 of 3 series passed |
| Screenshot 2024-09-15 131423.png | 1/6 | 2.19% | 7.10% | 12.24% | FAIL: 0 of 6 series passed |
| Screenshot 2024-09-15 131439.png | 1/2 | 2.41% | 10.39% | 4.08% | FAIL: 0 of 2 series passed |
| Screenshot 2024-09-15 131522.png | 1/8 | 36.43% | 41.77% | 0.00% | FAIL: 0 of 8 series passed |
| Screenshot 2024-09-15 131556.png | 0/14 | n/a | n/a | n/a | FAIL: review-extraction/score_extraction.py exited 1: ValueError: need at least one array to concatenate |
| Screenshot 2024-09-15 131624.png | 1/15 | 0.93% | 1.39% | 16.67% | FAIL: 0 of 15 series passed |
| Screenshot 2024-09-15 131643.png | 1/2 | 0.88% | 1.22% | 25.00% | FAIL: 0 of 2 series passed |
| Screenshot 2024-09-15 131712.png | 1/3 | 27.97% | 52.60% | 0.00% | FAIL: 0 of 3 series passed |
| Screenshot 2024-09-15 131740.png | 1/2 | 40.03% | 50.07% | 0.00% | FAIL: 0 of 2 series passed |
| Screenshot 2024-09-15 131805.png | 1/2 | 35.84% | 47.42% | 0.00% | FAIL: 0 of 2 series passed |

**Total:** 0 of 69 series passed.
