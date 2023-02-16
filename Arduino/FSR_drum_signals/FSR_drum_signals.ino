const int threshold = 150;
const int delayMs = 150;
const int drumCount = 2;
bool hits[drumCount];
int vals[drumCount];
int maxVals[drumCount];
unsigned long delayStart[drumCount];
int hitCount =0;

void setup() {
  // put your setup code here, to run once:
  for (bool &hit : hits) hit = false;
  for (int &maxVal : maxVals) maxVal = 0;
  Serial.begin(19200);
  
}
void loop() {
  // put your main code here, to run repeatedly:
  //Serial.print("off:"); 
  //Serial.println(0);  
  for (auto i =0;i<drumCount;i++) {
    vals[i] = analogRead(i);
    
    if (vals[i] > threshold && hits[i] == false) {
      Serial.print("on:"); 
      Serial.println(i);
      //Serial.print("count:"); 
      //Serial.println(hitCount);
      hitCount++;
      delayStart[i] = millis();
      hits[i] = true;
    } else if (hits[i]) {
      if (vals[i] > maxVals[i]) maxVals[i] = vals[i];
      if (millis()-delayStart[i] >= delayMs) {  
        //Serial.print("delay time: ");
        //Serial.println(millis()-delayStart[i]); 
        //Serial.print("max:");
        //Serial.println(maxVals[i]);
        //Serial.print(i);
        //Serial.println(" off"); 
        hits[i]=false;
        maxVals[i]=0;
      }
    }
  }
  

}
