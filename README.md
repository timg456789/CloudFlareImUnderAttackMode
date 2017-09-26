# CloudFlare I'm Under Attack Mode JavaScript Challenge Question

[CloudFlare Documentation](https://support.cloudflare.com/hc/en-us/articles/200170076-What-does-I-m-Under-Attack-Mode-do-)

>You will need to have both JavaScript and Cookies turned on in your browser to pass the check. The check is in place to make sure that you are not part of a botnet.


No JavaScript is executed in answering the challenge question in this project. Only mathematical operators and string parsing is used with no foreign code execution.

## Process
1. Visit `images.nga.gov`
2. Receive a 5xxx response and an HTML body with a challenge form. The challenge form has the answer obfuscated/encoded in JavaScript.
3. Parse the obfuscated JavaScript to extract the challenge question.
4. Decode the challenge question made up of JavaScript arrays e.g. `[]`
5. Wait 4 seconds for the server-side DDOS protection to clear the answer.
6. Receive a cf_clearance cookie from the challenge form response authorizing access to the home page and a 3xxx redirect to the home page.

*I don't know how long the cf_clearance cookie is valid for.*

**If you don't wait 4 seconds or pass the correct answer to get a cf_clearance token, you will continually be redirected to the challenge form until it is correctly filled out.**

### Clearance Token Cookie Sample

    cf_clearance=9f3c85ef0ab6bce1319ff978756dc1e0992e1be3-1505600061-28800
