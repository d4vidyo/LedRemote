#include <FastLED.h>
#include <WiFi.h>

//Led
#define ledPin 2
#define ledCount 300
CRGB leds[ledCount];

//Network
char* ssid = "Vodafone-113110";
char* pw = "6Argynxu2LLQL67q";
IPAddress ip(192, 168, 2, 69);
IPAddress gateway (192, 168, 2, 1);
IPAddress subnet(255, 255, 0, 0);
IPAddress DNS1(8, 8, 8, 8);
IPAddress DNS2(8, 8, 4, 4);
WiFiServer server(80);

//random
String answer = "Connected";
bool On = false;
int Mode = 0;
int Brightness = 0;
int Speed = 10;


void setup() {
  Serial.begin(115200);
  FastLED.addLeds<WS2812B, ledPin, GRB>(leds, ledCount);
  FastLED.setBrightness(Brightness);
  FastLED.show();

  setNetworkVivi();
  checkNetwork();
  connectNetwork();

}

void setNetworkDavid() {
  ssid = "Vodafone-113110";
  pw = "6Argynxu2LLQL67q";
}

void setNetworkVivi() {
  ssid = "o2-WLAN53";
  pw = "5264829999549261";
}

void checkNetwork() {
  Serial.println("");
  Serial.println("");
  int numSSid = WiFi.scanNetworks();
  if (numSSid == -1) {
    Serial.println("No Network detected");
  }

  for (int j = 0; j < numSSid; j++) {
    Serial.print(j);
    Serial.print(": ");
    Serial.println(WiFi.SSID(j));
  }
}

void connectNetwork() {
  Serial.print("connecting to: ");
  Serial.println(ssid);
  Serial.print("using pw: ");
  Serial.println(pw);

  WiFi.config(ip, gateway, subnet, DNS1, DNS2);
  WiFi.begin(ssid, pw);
  while (WiFi.status() != WL_CONNECTED) {
    //WiFi.begin(ssid, pw);
    Serial.println(WiFi.status());
    if (WiFi.status() == 1) {
      setNetworkDavid();
      delay(500);
      connectNetwork();
      return;
    }
    delay(500);
  }
  Serial.print("WiFi Connected. IP : ");
  Serial.println(WiFi.localIP());
  server.begin();
}

void interpretLine(String request) {
  if (request.endsWith("Ping")) {
    answer = "Connected";
  }
  if (request.endsWith("Status")) {
    answer = "Mode: " + (String)Mode;
    answer += "\n";
    answer += "Brightness: " + (String)Brightness;
    answer += "\n";
    answer += "Speed: " + (String)Speed;
  }
  if (request.endsWith("Off")) {
    On = false;
    answer = "Leds turned off";
  }
  if (request.endsWith("On")) {
    On = true;
    answer = "Leds turned on";
  }
  if (request.endsWith("Opacity")) {
    request.remove(0, 5);
    request.remove(4);
    Brightness = request.toFloat() * 2.55;
    FastLED.setBrightness(Brightness);
    answer = "Brightness set to: " + request;
  }
  if (request.endsWith("Mode")) {
    request.remove(0, 5);
    request.remove(1);
    Mode = request.toFloat();
    answer = "Mode set to " + (String)Mode;
  }
  FastLED.show();

}


void network() {
  WiFiClient client = server.available();   // listen for incoming clients

  if (client) {                             // if you get a client,
    Serial.println("New Client.");           // print a message out the serial port
    String currentLine = "";                // make a String to hold incoming data from the client
    while (client.connected()) {            // loop while the client's connected
      if (client.available()) {             // if there's bytes to read from the client,
        char c = client.read();             // read a byte, then
        Serial.write(c);                    // print it out the serial monitor
        if (c == '\n') {                    // if the byte is a newline character

          // if the current line is blank, you got two newline characters in a row.
          // that's the end of the client HTTP request, so send a response:
          if (currentLine.length() == 0) {
            // HTTP headers always start with a response code (e.g. HTTP/1.1 200 OK)
            // and a content-type so the client knows what's coming, then a blank line:
            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:text/html");
            client.println();

            // the content of the HTTP response follows the header:


            client.print(answer);


            // The HTTP response ends with another blank line:
            client.println();
            // break out of the while loop:
            break;
          }
          else {    // if you got a newline, then clear currentLine:
            currentLine = "";
          }
        } else if (c != '\r') {  // if you got anything else but a carriage return character,
          currentLine += c;      // add it to the end of the currentLine
        }

        interpretLine(currentLine);
      }
    }
    // close the connection:
    client.stop();
    Serial.println("Client Disconnected.");
  }
}


void hueRainbow() {
  static int hue = 0;
  static int Max = 1530;
  if (hue > Max) {
    hue = 0;
  }
  int saturation, value;
  for (int j = 0; j < ledCount; j++) {
    leds[j] = CHSV(hue / 6 - j, 255, 255);
  }
  hue++;
  FastLED.show();
  delay(10 / Speed);
}

void statickRainbow() {
  static int hue = 0;
  static int Max = 1530;
  if (hue > Max) {
    hue = 0;
  }
  for (int i = 0; i < ledCount; i++) {
    leds[i] = CHSV(hue, 255, 255);
  }
  FastLED.show();
  hue ++;
  delay(100 / Speed);
}


void SetLeds() {
  if (On) {
    switch (Mode) {
      case 1: hueRainbow();
        break;
      case 0: statickRainbow();
        break;
    }
  }
  else {
    for (int i = 0; i < ledCount; i++) {
      leds[i] = CRGB(0, 0, 0);
    }
    FastLED.show();
  }
}

void loop() {
  network();
  SetLeds();
}
