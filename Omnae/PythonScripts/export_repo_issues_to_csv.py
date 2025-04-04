"""
Exports Issues from a specified repository to a CSV file

Uses basic authentication (Github username + password) to retrieve Issues
from a repository that username has access to. Supports Github API v3.
"""
import csv
import requests


#Update your personal API token here
PERSONAL_TOKEN = 'D3df2b44ddcdd3516bc4c192d045ab3fd76b3b04'
headers = {'Authorization': 'token %s' % PERSONAL_TOKEN }

# Update your filter here.  Filter is who owns the issue.  State is open, closed, or all.
params_payload = {'filter' : 'all', 'state' : 'all' }

GITHUB_USER = 'hma14'
GITHUB_PASSWORD = 'Hma@1985'
REPO = 'omnae/Omnae'  # format is username/repo
ISSUES_FOR_REPO_URL = 'https://github.enterprise.com/api/v3/repos/%s/issues' % REPO
AUTH = (GITHUB_USER, GITHUB_PASSWORD)

def write_issues(response):
    "output a list of issues to csv"
    if not r.status_code == 200:
        raise Exception(r.status_code)
    for issue in r.json():
        labels = issue['labels']
        for label in labels:
            if label['name'] == "Client Requested":
                csvout.writerow([issue['number'], issue['title'].encode('utf-8'), issue['body'].encode('utf-8'), issue['created_at'], issue['updated_at']])


#r = requests.get(ISSUES_FOR_REPO_URL, auth=AUTH)
r = requests.get(ISSUES_FOR_REPO_URL, params=params_payload, headers=headers)
csvfile = '%s-issues.csv' % (REPO.replace('/', '-'))
csvout = csv.writer(open(csvfile, 'wb'))
csvout.writerow(('id', 'Title', 'Body', 'Created At', 'Updated At'))
write_issues(r)

#more pages? examine the 'link' header returned
if 'link' in r.headers:
    pages = dict(
        [(rel[6:-1], url[url.index('<')+1:-1]) for url, rel in
            [link.split(';') for link in
                r.headers['link'].split(',')]])
    while 'last' in pages and 'next' in pages:
        r = requests.get(pages['next'], auth=AUTH)
        write_issues(r)
        if pages['next'] == pages['last']:
            break
