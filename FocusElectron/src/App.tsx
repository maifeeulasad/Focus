import React, { useState, useEffect } from 'react'
import {Moment} from 'moment';
import electron from '/electron.png'
import react from '/react.svg'
import vite from '/vite.svg'
import styles from 'styles/app.module.scss'

import { readdir, readFile } from 'fs';

const App: React.FC = () => {
  const [logFiles, setLogFiles] = useState<string[]>([]);

  useEffect(()=>{
    const dir = "D:\\focus\\";
    readdir(dir,(_,files)=>{
      setLogFiles(files.map((file)=>dir+file))
    });
  },[]);

  const parseRecordsNode = (records: HTMLCollectionOf<Element>) => {
    // console.log(records);
    for(let record of records){
      // console.log(record)
      console.log(record.getElementsByTagName('process')[0].textContent);
      console.log(record.getElementsByTagName('timestamp')[0].textContent);
      // yyyy-MM-dd HH:mm:ss zzz
    }
  }

  const parseXml = (xmlString: string) => {
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(xmlString,"text/xml");


    xmlDoc.querySelectorAll("start,record,empty").forEach((node: Element)=>{
      console.log(node)
      console.log(node.nextElementSibling)
    })

    
    // for(let node in xmlDoc){
    //   console.log(node)
    // }
    // // start, record, empty
    // const starts = xmlDoc.getElementsByTagName('start');
    // const records = xmlDoc.getElementsByTagName('record');
    // const empties = xmlDoc.getElementsByTagName('empty');
    // return parseRecordsNode(records);
  }

  useEffect(()=>{
    console.log(logFiles);
    readFile(logFiles[0], 'utf8' ,(_,data)=>{
      parseXml(data);
    })
  },[logFiles])

  return (
    <div className={styles.app}>
      ping-pong
    </div>
  )
}

export default App
