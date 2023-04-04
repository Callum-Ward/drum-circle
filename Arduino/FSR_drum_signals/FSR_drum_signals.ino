
#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
#include <avr/power.h> // Required for 16 MHz Adafruit Trinket
#endif
#define PIN_NEO_PIXEL  4   // Arduino pin that connects to NeoPixel
#define NUM_PIXELS     30  // The number of LEDs (pixels) on NeoPixel
Adafruit_NeoPixel NeoPixel(NUM_PIXELS, PIN_NEO_PIXEL, NEO_GRB + NEO_KHZ800);

const int threshold = 920;
const int delayMs = 175;
const int peakDelay =35;
const int drumCount = 6;
bool hits[drumCount];
int vals[drumCount];
bool sent[drumCount];
int maxVals[drumCount];
unsigned long delayStart[drumCount];
int hitCount =0;

void setup() {
  // put your setup code here, to run once:
  for (int i=0;i<drumCount;i++) {
    hits[i] = false;
    maxVals[i] = 0;
    sent[i] = false;
  }
  Serial.begin(115200);

  NeoPixel.begin(); // INITIALIZE NeoPixel strip object (REQUIRED)
  Serial.println("Setup complete");
  
}

//brightness 0 to 255
void changeLeds(int on,int R,int G, int B) {
  if (on) {
    if (R==0 && G==0 && B==0) {
      int R = random(5,255);
      int G = random(5,255);
      int B = random(5,255);
    }
    // turn pixels to green one by one with delay between each pixel
    for (int pixel = 0; pixel < NUM_PIXELS; pixel++) { // for each pixel
      NeoPixel.setPixelColor(pixel, NeoPixel.Color(R, G, B)); // it only takes effect if pixels.show() is called
      NeoPixel.show();   // send the updated pixel colors to the NeoPixel hardware.
    }
  }else {
    NeoPixel.clear();
    NeoPixel.show();
  }

}

int getHitStrength(int hitVal) {
  if (hitVal > 920) {
    return 3;
  } else if (hitVal > 750) {
    return 2;
  } else {
    return 1;
  }
}

void loop() {
  NeoPixel.clear(); // set all pixel colors to 'off'. It only takes effect if pixels.show() is called
  // put your main code here, to run repeatedly:
  //Serial.print("off:"); 
  //Serial.println(0);  
  for (auto i =0;i<drumCount;i++) {
    
    vals[i] = analogRead(i);
    if (vals[i] > threshold && hits[i] == false) {
      maxVals[i]=vals[i];
      hitCount++;
      hits[i] = true;
      changeLeds(1,0,0,0);
      delayStart[i] = millis();
    } else if (hits[i]) {
      
      if (vals[i] > maxVals[i] && !sent[i]) maxVals[i] = vals[i];

      if ( millis()-delayStart[i] >= peakDelay && !sent[i]) {
        sent[i] = true;
        Serial.print("on:"); 
        Serial.print(i);
        Serial.print(":s:");
        Serial.println(maxVals[i]);
        maxVals[i]=0;

        //Serial.println(getHitStrength(maxVals[i]));
      }

      if (millis()-delayStart[i] >= delayMs && vals[i] < threshold) {  
        hits[i]=false;
        sent[i]=false;
        maxVals[i]=0;
        changeLeds(0,0,0,0);
      }
    }
  }
}
