import CoreGraphics
import AVFoundation
import Vision
import ARKit
import CoreVideo


@objc public class HandDetector :NSObject {

    @objc public static let shared = HandDetector()
    private var handPoseRequest = VNDetectHumanHandPoseRequest()
    public override init() {
        print("HandGestureProcessor init")
    }
    
    @objc public func DetectHand(buffer: CVPixelBuffer) -> String{
        print("DetectHand")
        let handler = VNImageRequestHandler(cvPixelBuffer: buffer, orientation: .up, options: [:])
        do {
            handPoseRequest.maximumHandCount = 1
            var res = ""
            try handler.perform([handPoseRequest])
            guard let observation = handPoseRequest.results?.first else {
                return ""
            }

            let allPoints = try observation.recognizedPoints(.all)
            for item in allPoints {
                res+="\(item.value.identifier.rawValue),\(item.value.x),\(item.value.y),\(item.value.confidence)"+"\r"
            }
            
            return res
        } catch {
            //let error = AppError.visionError(error: error)
            //print(error)
        }
        
        return ""
    }

    @objc public func DetectMultiHand(buffer: CVPixelBuffer,maximumHandCount:Int) -> String{
        print("DetectMultiHand")
        let handler = VNImageRequestHandler(cvPixelBuffer: buffer, orientation: .up, options: [:])
        do {
            handPoseRequest.maximumHandCount = maximumHandCount
            var res = ""
            try handler.perform([handPoseRequest])
            if let total = handPoseRequest.results?.count {
                var index = 0
                while index<total {
                    if let observation = handPoseRequest.results?[index] {
                        let allPoints = try observation.recognizedPoints(.all)
                        for item in allPoints {
                            res+="\(item.value.identifier.rawValue),\(item.value.x),\(item.value.y),\(item.value.confidence)"+"\r"
                        }
                    }
                    index = index + 1
                }
            }
            
            return res
        } catch {
            //let error = AppError.visionError(error: error)
            //print(error)
        }
        
        return ""
    }
    
}

