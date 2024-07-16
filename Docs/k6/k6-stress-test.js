import http from 'k6/http';
import { sleep, check } from 'k6';
import { Counter } from 'k6/metrics';

// Define custom metrics
const myFailingCounter = new Counter('my_failing_counter');

const startDate = new Date(2024, 6, 16);
const endDate = new Date(2024, 6, 18);
const myHost = 'http://localhost:5000';

export let options = {
    InsecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        { duration: '1m', target: 10 },
        { duration: '2m', target: 50 },
        { duration: '2m', target: 100 },
        { duration: '5m', target: 200 },
        { duration: '5m', target: 400 },
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
    },
};

function getRandomDate(startDate, endDate) {
    // Convert the dates to timestamps
    const startTimestamp = startDate.getTime();
    const endTimestamp = endDate.getTime();

    // Generate a random timestamp between the start and end dates
    const randomTimestamp = Math.floor(Math.random() * (endTimestamp - startTimestamp + 1) + startTimestamp);

    // Create a new Date object from the random timestamp
    return new Date(randomTimestamp);
}

export default function () {
    const randomDate = getRandomDate(startDate, endDate).toISOString();

    const url = `${myHost}/api/v1/web-hook/create`;
    let payload = JSON.stringify({
        uri: 'https://httpbin.org/post',
        httpMethod: 2,
        numberRetry: 1,
        enableVerifyTls: true,
        triggerDatetimeUtc: randomDate,
    });

    let params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    let res = http.post(url, payload, params);

    // Check for successful response
    let success = check(res, {
        'status is 200': (r) => r.status === 200,
    });

    // Count failed requests
    if (!success) {
        myFailingCounter.add(1);
    }

    // Sleep to pace requests
    sleep(1);
}
