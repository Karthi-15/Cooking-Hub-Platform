import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EncryptPasswordService {
  private key = CryptoJS.enc.Utf8.parse(environment.encryptionKey);
  private iv = CryptoJS.enc.Utf8.parse(environment.ivKey);

  encrypt(password: string): string {
    const encrypted = CryptoJS.AES.encrypt(password, this.key, {
      iv: this.iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });
  
    // Combine ciphertext + IV into a single Base64 string
    const encryptedHex = encrypted.ciphertext.toString(CryptoJS.enc.Hex);
    const encryptedBase64 = CryptoJS.enc.Base64.stringify(CryptoJS.enc.Hex.parse(encryptedHex));
    return encryptedBase64;
  }
  
  
}
